-- Таблица Сервисы
CREATE TABLE services (
	service_code SERIAL PRIMARY KEY,
	name TEXT NOT NULL,
	UNIQUE (name)
);

-- Таблица Отделы Служб
CREATE TABLE departments (
	service_code INT NOT NULL,
	dept_code SERIAL NOT NULL,
	name TEXT NOT NULL,
	address TEXT,
	PRIMARY KEY (service_code, dept_code),
	FOREIGN KEY (service_code) REFERENCES services(service_code) ON DELETE CASCADE
);

-- Таблица Участки
CREATE TABLE sections (
	service_code INT NOT NULL,
	dept_code INT NOT NULL,
	section_code SERIAL NOT NULL,
	name TEXT NOT NULL,
	PRIMARY KEY (service_code, dept_code, section_code),
	FOREIGN KEY (service_code, dept_code) REFERENCES departments(service_code, dept_code) ON DELETE CASCADE
);

-- Таблица Дома
CREATE TABLE houses (
	house_code SERIAL PRIMARY KEY,
	service_code INT NOT NULL,
	dept_code INT NOT NULL,
	section_code INT NOT NULL,
	street TEXT NOT NULL,
	house_number VARCHAR(20) NOT NULL,
	building VARCHAR(20),
	FOREIGN KEY (service_code, dept_code, section_code) REFERENCES sections(service_code, dept_code, section_code) ON DELETE CASCADE,
	UNIQUE (service_code, dept_code, section_code, street, house_number, building)
);

-- Таблица шифров плательщика
CREATE TABLE payer_codes (
	payer_code SERIAL PRIMARY KEY,
	name TEXT NOT NULL,
	percent_share NUMERIC(5, 2) DEFAULT 100 CHECK (percent_share >= 0 AND percent_share <= 100)
);

-- Таблица Тарифов
CREATE TABLE tariffs (
	tariff_code SERIAL PRIMARY KEY,
	cold_water BOOLEAN NOT NULL DEFAULT true,
	hot_water BOOLEAN NOT NULL DEFAULT false,
	garbage_chute BOOLEAN NOT NULL DEFAULT true,
	elevator BOOLEAN NOT NULL DEFAULT true,
	rate NUMERIC(10, 2) NOT NULL CHECK (rate >= 0),
	UNIQUE (cold_water, hot_water, garbage_chute, elevator, rate)
);

-- Таблица Квартир
CREATE TABLE apartments (
	apartment_code SERIAL PRIMARY KEY,
	house_code INT NOT NULL REFERENCES houses(house_code) ON DELETE CASCADE,
	apt_number VARCHAR(20) NOT NULL,
	living_area NUMERIC(8, 2) CHECK (living_area >= 0) DEFAULT 0,
	total_area NUMERIC(8, 2) CHECK (total_area >= 0) DEFAULT 0,
	privatized BOOLEAN NOT NULL DEFAULT false,
	cold_water BOOLEAN NOT NULL DEFAULT true,
	hot_water BOOLEAN NOT NULL DEFAULT false,
	garbage_chute BOOLEAN NOT NULL DEFAULT true,
	elevator BOOLEAN NOT NULL DEFAULT true,
	tariff_id INT REFERENCES tariffs(tariff_code),
	payer_code INT REFERENCES payer_codes(payer_code),
	last_calculated_charge NUMERIC(12, 2) DEFAULT 0,
	UNIQUE (house_code, apt_number)
);

-- Таблица Жильцов
CREATE TABLE residents (
	resident_code SERIAL PRIMARY KEY,
	apartment_code INT NOT NULL REFERENCES apartments (apartment_code) ON DELETE CASCADE,
	full_name VARCHAR(250) NOT NULL,
	inn VARCHAR(12),
	passport TEXT,
	birth_date DATE,
	is_responsible BOOLEAN NOT NULL DEFAULT false,
	payer_code INT REFERENCES payer_codes(payer_code),
	UNIQUE (apartment_code, full_name)
);

-- Таблица Платежи
CREATE TABLE payments (
	payment_code SERIAL PRIMARY KEY,
	apartment_code INT NOT NULL REFERENCES apartments (apartment_code) ON DELETE CASCADE,
	resident_code INT REFERENCES residents (resident_code) ON DELETE SET NULL,
	tariff_code INT REFERENCES tariffs (tariff_code) ON DELETE SET NULL,
	amount NUMERIC(12, 2) NOT NULL CHECK (amount >= 0),
	pay_date DATE NOT NULL DEFAULT current_date,
	period_month DATE NOT NULL
);

-- Денормализированная таблица платежей по квартире
CREATE TABLE apartment_payments_summary (
	apartment_code INT PRIMARY KEY REFERENCES apartments (apartment_code) ON DELETE CASCADE,
	total_paid NUMERIC(14, 2) NOT NULL DEFAULT 0 CHECK (total_paid >= 0),
	last_payment_date DATE
);

-- Инициализируем summary для уже существующих квартир (вставит пустые строки, если апартаменты есть)
INSERT INTO apartment_payments_summary (apartment_code)
	SELECT apartment_code FROM apartments
	ON CONFLICT (apartment_code) DO NOTHING;

-- Индексы
CREATE INDEX IF NOT EXISTS idx_houses_service_dept ON houses(service_code, dept_code);
CREATE INDEX IF NOT EXISTS idx_apartments_house ON apartments(house_code);
CREATE INDEX IF NOT EXISTS idx_residents_apt ON residents(apartment_code);
CREATE INDEX IF NOT EXISTS idx_payments_apt_period ON payments(apartment_code, period_month);
CREATE INDEX IF NOT EXISTS idx_payments_resident ON payments(resident_code);

-- Триггер: чтобы только один 'is_responsible' в квартире был true
CREATE OR REPLACE FUNCTION trg_resident_one_responsible()
RETURNS TRIGGER AS $$
BEGIN
	IF (NEW.is_responsible IS TRUE) THEN
		UPDATE residents
		SET is_responsible = FALSE
		WHERE apartment_code = NEW.apartment_code
		  AND COALESCE(resident_code, 0) <> COALESCE(NEW.resident_code, 0);
	END IF;
	RETURN NEW;
END;
$$ LANGUAGE plpgsql;

DROP TRIGGER IF EXISTS resident_one_responsible_before ON residents;
CREATE TRIGGER resident_one_responsible_before
	BEFORE INSERT OR UPDATE ON residents
	FOR EACH ROW EXECUTE FUNCTION trg_resident_one_responsible();

-- Триггер: поддержка apartment_payments_summary
CREATE OR REPLACE FUNCTION trg_payments_update_summary()
RETURNS TRIGGER AS $$
DECLARE
	max_date DATE;
BEGIN
	-- INSERT
	IF TG_OP = 'INSERT' THEN
		INSERT INTO apartment_payments_summary(apartment_code, total_paid, last_payment_date)
		VALUES (NEW.apartment_code, NEW.amount, NEW.pay_date)
		ON CONFLICT (apartment_code) DO UPDATE
			SET total_paid = apartment_payments_summary.total_paid + NEW.amount,
			    last_payment_date = GREATEST(COALESCE(apartment_payments_summary.last_payment_date, DATE '0001-01-01'), NEW.pay_date);
		RETURN NEW;
	END IF;

	-- UPDATE
	IF TG_OP = 'UPDATE' THEN
		IF OLD.apartment_code IS DISTINCT FROM NEW.apartment_code THEN
			UPDATE apartment_payments_summary
			SET total_paid = GREATEST(total_paid - OLD.amount, 0)
			WHERE apartment_code = OLD.apartment_code;
			SELECT MAX(pay_date) INTO max_date FROM payments WHERE apartment_code = OLD.apartment_code;
			UPDATE apartment_payments_summary
			SET last_payment_date = max_date
			WHERE apartment_code = OLD.apartment_code;
			INSERT INTO apartment_payments_summary(apartment_code, total_paid, last_payment_date)
			VALUES (NEW.apartment_code, NEW.amount, NEW.pay_date)
			ON CONFLICT (apartment_code) DO UPDATE
				SET total_paid = apartment_payments_summary.total_paid + NEW.amount,
				    last_payment_date = GREATEST(COALESCE(apartment_payments_summary.last_payment_date, DATE '0001-01-01'), NEW.pay_date);
		ELSE
			UPDATE apartment_payments_summary
			SET total_paid = GREATEST(total_paid - OLD.amount + NEW.amount, 0)
			WHERE apartment_code = NEW.apartment_code;

			SELECT MAX(pay_date) INTO max_date FROM payments WHERE apartment_code = NEW.apartment_code;
			UPDATE apartment_payments_summary
			SET last_payment_date = max_date
			WHERE apartment_code = NEW.apartment_code;
		END IF;
		RETURN NEW;
	END IF;

	-- DELETE
	IF TG_OP = 'DELETE' THEN
		UPDATE apartment_payments_summary
		SET total_paid = GREATEST(total_paid - OLD.amount, 0)
		WHERE apartment_code = OLD.apartment_code;
		SELECT MAX(pay_date) INTO max_date FROM payments WHERE apartment_code = OLD.apartment_code;
		UPDATE apartment_payments_summary
		SET last_payment_date = max_date
		WHERE apartment_code = OLD.apartment_code;
		RETURN OLD;
	END IF;

	RETURN NULL;
END;
$$ LANGUAGE plpgsql;

DROP TRIGGER IF EXISTS payments_after_insert ON payments;
CREATE TRIGGER payments_after_insert
	AFTER INSERT ON payments
	FOR EACH ROW EXECUTE FUNCTION trg_payments_update_summary();

DROP TRIGGER IF EXISTS payments_after_update ON payments;
CREATE TRIGGER payments_after_update
	AFTER UPDATE ON payments
	FOR EACH ROW EXECUTE FUNCTION trg_payments_update_summary();

DROP TRIGGER IF EXISTS payments_after_delete ON payments;
CREATE TRIGGER payments_after_delete
	AFTER DELETE ON payments
	FOR EACH ROW EXECUTE FUNCTION trg_payments_update_summary();
	
-- VIEW
-- тарифы с горячей водой
CREATE OR REPLACE VIEW v_hot_water_tariffs AS
	SELECT tariff_code, rate
	FROM tariffs
	WHERE hot_water = true;

-- VIEW по квартире
CREATE OR REPLACE VIEW v_apartment_full_info AS
	SELECT a.apartment_code,
			h.street, h.house_number, h.building,
			a.apt_number,
			a.living_area, a.total_area,
			r.resident_code AS responsible_code,
			r.full_name AS responsible_name,
			aps.total_paid,
			aps.last_payment_date
		FROM apartments a
		JOIN houses h ON a.house_code = h.house_code
		LEFT JOIN residents r ON r.apartment_code = a.apartment_code AND r.is_responsible = true
		LEFT JOIN apartment_payments_summary aps ON aps.apartment_code = a.apartment_code;

-- участки (> 10 жителей)
CREATE OR REPLACE VIEW v_sections_overloaded AS
	SELECT sec.service_code, sec.dept_code, sec.section_code, sec.name AS section_name,
		   COUNT(r.resident_code) AS residents_count
	FROM sections sec
	JOIN houses h ON h.service_code = sec.service_code AND h.dept_code = sec.dept_code AND h.section_code = sec.section_code
	JOIN apartments a ON a.house_code = h.house_code
	JOIN residents r ON r.apartment_code = a.apartment_code
	GROUP BY sec.service_code, sec.dept_code, sec.section_code, sec.name
	HAVING COUNT(r.resident_code) > 10;
	
	
	
-- Службы
INSERT INTO services (name) VALUES
('ЖКХ'), ('Электросеть');

-- Отделы
INSERT INTO departments (service_code, name, address) VALUES
(1, 'Отдел 1 ЖКХ', 'ул. Ленина, д. 1'),
(1, 'Отдел 2 ЖКХ', 'ул. Гагарина, д. 10'),
(2, 'Отдел 1 Электросеть', 'ул. Пушкина, д. 5'),
(2, 'Отдел 2 Электросеть', 'ул. Чехова, д. 7');

-- Участки
INSERT INTO sections (service_code, dept_code, name) VALUES
(1, 1, 'Участок 1 ЖКХ'),
(1, 2, 'Участок 2 ЖКХ'),
(2, 3, 'Участок 1 Электросеть'),
(2, 4, 'Участок 2 Электросеть');

-- Дома
INSERT INTO houses (service_code, dept_code, section_code, street, house_number, building) VALUES
(1, 1, 1, 'ул. Ленина', '10', NULL),
(1, 1, 1, 'ул. Ленина', '12', NULL),
(1, 2, 2, 'ул. Гагарина', '20', 'А'),
(1, 2, 2, 'ул. Гагарина', '22', 'Б'),
(2, 3, 3, 'ул. Пушкина', '5', NULL),
(2, 4, 4, 'ул. Чехова', '7', NULL);

-- =========================
-- Плательщики
INSERT INTO payer_codes (name, percent_share) VALUES
('Основной плательщик', 100),
('Сосед по квартире', 50);

-- Тарифы
INSERT INTO tariffs (cold_water, hot_water, garbage_chute, elevator, rate) VALUES
(true, true, true, true, 3000),
(true, false, true, true, 2500),
(false, false, true, false, 2000);

-- Квартиры
INSERT INTO apartments (house_code, apt_number, living_area, total_area, privatized, cold_water, hot_water, garbage_chute, elevator, tariff_id, payer_code)
VALUES
(1, '1', 50, 70, true, true, true, true, true, 1, 1),
(1, '2', 45, 65, true, true, true, true, true, 1, 1),
(1, '3', 40, 60, false, true, false, true, true, 2, 2),
(2, '1', 55, 75, true, true, true, true, true, 1, 1),
(2, '2', 48, 68, true, true, true, true, true, 1, 1),
(3, '1', 60, 80, true, true, false, true, true, 2, 1),
(3, '2', 42, 62, false, true, false, true, false, 3, 2);

-- Жильцы
INSERT INTO residents (apartment_code, full_name, inn, passport, birth_date, is_responsible, payer_code) VALUES
(1, 'Иванов Иван Иванович', '123456789012', 'AA123456', '1980-05-12', true, 1),
(1, 'Петров Петр Петрович', '234567890123', 'BB234567', '1985-08-23', false, 2),
(2, 'Сидоров Сидор Сидорович', '345678901234', 'CC345678', '1990-01-15', true, 1),
(3, 'Кузнецов Алексей', '456789012345', 'DD456789', '1975-07-07', true, 1),
(3, 'Морозова Анна', '567890123456', 'EE567890', '1982-03-19', false, 2);

-- Платежи
INSERT INTO payments (apartment_code, resident_code, tariff_code, amount, pay_date, period_month) VALUES
(1, 1, 1, 3000, '2025-10-01', '2025-10-01'),
(1, 2, 1, 1500, '2025-10-03', '2025-10-01'),
(2, 3, 1, 3000, '2025-10-05', '2025-10-01'),
(3, 4, 2, 2500, '2025-10-07', '2025-10-01');