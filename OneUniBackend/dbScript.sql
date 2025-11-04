-- Create Database (run this separately first)
-- CREATE DATABASE oneuni;

-- Step 2: Extensions and ENUMs (MUST be first)
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

CREATE TYPE user_role AS ENUM ('student', 'mentor', 'university_representative', 'admin');
CREATE TYPE gender_type AS ENUM ('male', 'female', 'other', 'prefer_not_to_say');
CREATE TYPE education_type AS ENUM ('matric', 'intermediate', 'a_levels', 'o_levels', 'equivalent');
CREATE TYPE test_type AS ENUM ('NET', 'ECAT', 'MDCAT', 'SAT', 'IELTS', 'TOEFL', 'FAST', 'LUMS', 'other');
CREATE TYPE application_status AS ENUM ('draft', 'scheduled', 'submitted', 'under_review', 'await_merit_list', 'awaiting_fee_submission', 'accepted', 'rejected');
CREATE TYPE session_type AS ENUM ('free', 'paid');
CREATE TYPE session_status AS ENUM ('scheduled', 'completed', 'cancelled', 'no_show');
CREATE TYPE document_type AS ENUM ('matric_certificate', 'intermediate_certificate', 'transcript', 'cnic', 'passport', 'nicop', 'b_form', 'sports_certificate', 'hafiz_certificate', 'income_certificate', 'domicile', 'other');
CREATE TYPE verification_status AS ENUM ('pending', 'verified', 'rejected');
CREATE TYPE guardian_relation AS ENUM ('father', 'mother', 'guardian', 'other');
CREATE TYPE id_document_type AS ENUM ('cnic', 'nicop', 'passport', 'b_form');

-- Step 3: Base Tables (no dependencies)
CREATE TABLE users (
    user_id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    email VARCHAR(255) UNIQUE NOT NULL,
    password_hash VARCHAR(255) NOT NULL,
    role user_role NOT NULL,
    full_name VARCHAR(255),
    is_active BOOLEAN DEFAULT true,
    is_verified BOOLEAN DEFAULT false,
    verification_token VARCHAR(255),
    password_reset_token VARCHAR(255),
    password_reset_expires TIMESTAMPTZ,
    last_login TIMESTAMPTZ,
    created_at TIMESTAMPTZ DEFAULT (NOW() AT TIME ZONE 'UTC'),
    updated_at TIMESTAMPTZ DEFAULT (NOW() AT TIME ZONE 'UTC'),
    deleted_at TIMESTAMPTZ NULL
);

CREATE TABLE universities (
    university_id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    name VARCHAR(255) NOT NULL,
    short_name VARCHAR(20),
    type VARCHAR(50),
    city VARCHAR(100),
    province VARCHAR(50),
    country VARCHAR(50) DEFAULT 'Pakistan',
    established_year INTEGER,
    website_url VARCHAR(500),
    email VARCHAR(255),
    phone VARCHAR(20),
    address TEXT,
    logo_url VARCHAR(500),
    ranking_national INTEGER,
    ranking_international INTEGER,
    accreditation TEXT,
    is_active BOOLEAN DEFAULT true,
    created_at TIMESTAMPTZ DEFAULT (NOW() AT TIME ZONE 'UTC'),
    updated_at TIMESTAMPTZ DEFAULT (NOW() AT TIME ZONE 'UTC')
);

-- Step 4: Level 1 Dependencies
CREATE TABLE student_profiles (
    profile_id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    user_id UUID REFERENCES users(user_id) ON DELETE CASCADE,
    gender gender_type,
    date_of_birth DATE,
    id_document_type id_document_type,
    cnic VARCHAR(15), -- Also used for B-form numbers
    passport_number VARCHAR(20), -- For international students
    nicop_number VARCHAR(20), -- For overseas Pakistanis
    guardian_name VARCHAR(255),
    guardian_relation guardian_relation,
    city VARCHAR(100),
    address TEXT,
    phone VARCHAR(20),
    profile_picture_url VARCHAR(500),
    scholarship_priority BOOLEAN DEFAULT false,
    hostel_priority BOOLEAN DEFAULT false,
    preferred_admission_city VARCHAR(100),
    household_income DECIMAL(12,2),
    is_hafiz_quran BOOLEAN DEFAULT false,
    is_disabled BOOLEAN DEFAULT false,
    sports VARCHAR(100),
    profile_completed BOOLEAN DEFAULT false,
    completion_percentage INTEGER DEFAULT 0,
    deleted_at TIMESTAMPTZ NULL
);

CREATE TABLE educational_records (
    record_id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    user_id UUID REFERENCES users(user_id) ON DELETE CASCADE,
    education_type education_type NOT NULL,
    institution_name VARCHAR(255),
    board_university VARCHAR(255),
    roll_number VARCHAR(50),
    total_marks INTEGER,
    obtained_marks INTEGER,
    percentage DECIMAL(5,2),
    grade VARCHAR(10),
    year_of_completion INTEGER,
    is_result_awaited BOOLEAN DEFAULT false,
    created_at TIMESTAMPTZ DEFAULT (NOW() AT TIME ZONE 'UTC'),
    updated_at TIMESTAMPTZ DEFAULT (NOW() AT TIME ZONE 'UTC')
);

CREATE TABLE test_scores (
    score_id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    user_id UUID REFERENCES users(user_id) ON DELETE CASCADE,
    test_type test_type NOT NULL,
    test_name VARCHAR(255),
    total_marks INTEGER,
    obtained_marks INTEGER,
    percentage DECIMAL(5,2),
    test_date DATE,
    year INTEGER NOT NULL,
    roll_number VARCHAR(50),
    created_at TIMESTAMPTZ DEFAULT (NOW() AT TIME ZONE 'UTC'),
    updated_at TIMESTAMPTZ DEFAULT (NOW() AT TIME ZONE 'UTC')
);

CREATE TABLE departments (
    department_id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    university_id UUID REFERENCES universities(university_id) ON DELETE CASCADE,
    name VARCHAR(255) NOT NULL,
    short_name VARCHAR(20),
    description TEXT,
    head_of_department VARCHAR(255),
    email VARCHAR(255),
    phone VARCHAR(20),
    is_active BOOLEAN DEFAULT true,
    created_at TIMESTAMPTZ DEFAULT (NOW() AT TIME ZONE 'UTC'),
    updated_at TIMESTAMPTZ DEFAULT (NOW() AT TIME ZONE 'UTC')
);

CREATE TABLE admission_cycles (
    cycle_id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    university_id UUID REFERENCES universities(university_id) ON DELETE CASCADE,
    academic_year VARCHAR(10) NOT NULL,
    session_name VARCHAR(50),
    application_start_date DATE,
    application_end_date DATE,
    test_date DATE,
    merit_list_date DATE,
    fee_submission_deadline DATE,
    classes_start_date DATE,
    is_active BOOLEAN DEFAULT true,
    created_at TIMESTAMPTZ DEFAULT (NOW() AT TIME ZONE 'UTC'),
    updated_at TIMESTAMPTZ DEFAULT (NOW() AT TIME ZONE 'UTC')
);

CREATE TABLE university_representatives (
    rep_id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    user_id UUID REFERENCES users(user_id) ON DELETE CASCADE,
    university_id UUID REFERENCES universities(university_id) ON DELETE CASCADE,
    designation VARCHAR(255),
    department VARCHAR(255),
    phone VARCHAR(20),
    office_address TEXT,
    is_official BOOLEAN DEFAULT false,
    verification_status verification_status DEFAULT 'pending',
    verification_document_url VARCHAR(500),
    permissions JSONB,
    is_active BOOLEAN DEFAULT true
);

CREATE TABLE mentors (
    mentor_id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    user_id UUID REFERENCES users(user_id) ON DELETE CASCADE,
    designation VARCHAR(255),
    current_institution VARCHAR(255),
    graduation_year INTEGER,
    field_of_study VARCHAR(255),
    experience_years INTEGER,
    phone VARCHAR(20),
    linkedin_url VARCHAR(500),
    bio TEXT,
    profile_picture_url VARCHAR(500),
    specializations TEXT[],
    hourly_rate DECIMAL(8,2),
    availability_hours JSONB,
    university_email VARCHAR(255),
    university_id UUID REFERENCES universities(university_id),
    verification_status verification_status DEFAULT 'pending',
    total_sessions INTEGER DEFAULT 0,
    average_rating DECIMAL(3,2) DEFAULT 0,
    is_active BOOLEAN DEFAULT true
);

CREATE TABLE scholarships (
    scholarship_id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    university_id UUID REFERENCES universities(university_id) ON DELETE CASCADE,
    name VARCHAR(255) NOT NULL,
    description TEXT,
    minimum_merit DECIMAL(8,4),
    income_criteria DECIMAL(12,2),
    region_specific VARCHAR(100),
    coverage_percentage DECIMAL(5,2),
    coverage_amount DECIMAL(12,2),
    additional_benefits TEXT,
    application_deadline DATE,
    academic_year VARCHAR(10),
    is_active BOOLEAN DEFAULT true,
    created_at TIMESTAMPTZ DEFAULT (NOW() AT TIME ZONE 'UTC'),
    updated_at TIMESTAMPTZ DEFAULT (NOW() AT TIME ZONE 'UTC')
);

-- Step 5: Level 2 Dependencies
CREATE TABLE programs (
    program_id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    department_id UUID REFERENCES departments(department_id) ON DELETE CASCADE,
    name VARCHAR(255) NOT NULL,
    degree_type VARCHAR(50) DEFAULT 'BS',
    duration_years INTEGER DEFAULT 4,
    total_credit_hours INTEGER,
    description TEXT,
    eligibility_criteria TEXT,
    is_active BOOLEAN DEFAULT true,
    created_at TIMESTAMPTZ DEFAULT (NOW() AT TIME ZONE 'UTC'),
    updated_at TIMESTAMPTZ DEFAULT (NOW() AT TIME ZONE 'UTC')
);

-- Step 6: Level 3 Dependencies
CREATE TABLE merit_formulas (
    formula_id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    university_id UUID REFERENCES universities(university_id) ON DELETE CASCADE,
    program_id UUID REFERENCES programs(program_id) ON DELETE SET NULL,
    name VARCHAR(255) NOT NULL,
    test_weightage DECIMAL(5,2),
    intermediate_weightage DECIMAL(5,2),
    matric_weightage DECIMAL(5,2),
    interview_weightage DECIMAL(5,2) DEFAULT 0,
    required_test_types test_type[],
    minimum_test_score DECIMAL(5,2),
    minimum_intermediate_percentage DECIMAL(5,2),
    minimum_matric_percentage DECIMAL(5,2),
    academic_year VARCHAR(10),
    is_active BOOLEAN DEFAULT true,
    created_at TIMESTAMPTZ DEFAULT (NOW() AT TIME ZONE 'UTC'),
    updated_at TIMESTAMPTZ DEFAULT (NOW() AT TIME ZONE 'UTC')
);

CREATE TABLE applications (
    application_id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    user_id UUID REFERENCES users(user_id) ON DELETE CASCADE,
    university_id UUID REFERENCES universities(university_id) ON DELETE CASCADE,
    program_id UUID REFERENCES programs(program_id) ON DELETE CASCADE,
    cycle_id UUID REFERENCES admission_cycles(cycle_id) ON DELETE SET NULL,
    application_status application_status DEFAULT 'draft',
    application_number VARCHAR(50),
    submission_date TIMESTAMPTZ,
    scholarship_applied BOOLEAN DEFAULT false,
    hostel_required BOOLEAN DEFAULT false,
    transport_required BOOLEAN DEFAULT false,
    calculated_merit DECIMAL(8,4),
    merit_position INTEGER,
    fee_challan_number VARCHAR(100),
    fee_paid_amount DECIMAL(12,2),
    fee_payment_date DATE,
    admission_offered BOOLEAN DEFAULT false,
    offer_date DATE,
    offer_expires_at DATE,
    rejection_reason TEXT,
    scheduled_submission_date DATE,
    auto_submitted BOOLEAN DEFAULT false,
    created_at TIMESTAMPTZ DEFAULT (NOW() AT TIME ZONE 'UTC'),
    updated_at TIMESTAMPTZ DEFAULT (NOW() AT TIME ZONE 'UTC'),
    deleted_at TIMESTAMPTZ NULL
);

-- Step 7: Level 4 Dependencies  
CREATE TABLE documents (
    document_id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    user_id UUID REFERENCES users(user_id) ON DELETE CASCADE,
    application_id UUID REFERENCES applications(application_id) ON DELETE SET NULL,
    document_type document_type NOT NULL,
    document_name VARCHAR(255) NOT NULL,
    file_path VARCHAR(500) NOT NULL,
    file_size INTEGER,
    mime_type VARCHAR(100),
    verification_status verification_status DEFAULT 'pending',
    verified_by UUID REFERENCES users(user_id) ON DELETE SET NULL,
    verified_at TIMESTAMPTZ,
    rejection_reason TEXT,
    is_required BOOLEAN DEFAULT false,
    display_order INTEGER DEFAULT 0,
    created_at TIMESTAMPTZ DEFAULT (NOW() AT TIME ZONE 'UTC'),
    updated_at TIMESTAMPTZ DEFAULT (NOW() AT TIME ZONE 'UTC')
);

CREATE TABLE mentorship_sessions (
    session_id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    mentor_id UUID REFERENCES mentors(mentor_id) ON DELETE CASCADE,
    student_id UUID REFERENCES users(user_id) ON DELETE CASCADE,
    session_type session_type NOT NULL,
    scheduled_at TIMESTAMPTZ NOT NULL,
    duration_minutes INTEGER DEFAULT 60,
    session_status session_status DEFAULT 'scheduled',
    actual_start_time TIMESTAMPTZ,
    actual_end_time TIMESTAMPTZ,
    topic VARCHAR(255),
    session_notes TEXT,
    mentor_feedback TEXT,
    student_feedback TEXT,
    fee_amount DECIMAL(8,2),
    payment_status VARCHAR(50),
    payment_reference VARCHAR(100),
    mentor_rating INTEGER CHECK (mentor_rating BETWEEN 1 AND 5),
    student_rating INTEGER CHECK (student_rating BETWEEN 1 AND 5),
    created_at TIMESTAMPTZ DEFAULT (NOW() AT TIME ZONE 'UTC'),
    updated_at TIMESTAMPTZ DEFAULT (NOW() AT TIME ZONE 'UTC')
);

-- Step 8: Authentication Tables (NEW)
-- For Google OAuth / external logins
CREATE TABLE user_logins (
    LoginProvider VARCHAR(128) NOT NULL,
    ProviderKey VARCHAR(128) NOT NULL,
    ProviderDisplayName TEXT NULL,
    UserId UUID NOT NULL,
    CONSTRAINT PK_user_logins PRIMARY KEY (LoginProvider, ProviderKey),
    CONSTRAINT FK_user_logins_users_UserId FOREIGN KEY (UserId) REFERENCES users(user_id) ON DELETE CASCADE
);
CREATE INDEX IX_user_logins_UserId ON user_logins(UserId);

-- For storing our API refresh tokens
CREATE TABLE user_refresh_tokens (
    token_id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    user_id UUID NOT NULL REFERENCES users(user_id) ON DELETE CASCADE,
    token_hash VARCHAR(255) NOT NULL,
    created_at TIMESTAMPTZ DEFAULT (NOW() AT TIME ZONE 'UTC'),
    expires_at TIMESTAMPTZ NOT NULL,
    is_revoked BOOLEAN DEFAULT false
);
CREATE INDEX idx_user_refresh_tokens_user_id ON user_refresh_tokens(user_id);


-- Step 9: Supporting Tables (Original)
CREATE TABLE notifications (
    notification_id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    user_id UUID REFERENCES users(user_id) ON DELETE CASCADE,
    title VARCHAR(255) NOT NULL,
    message TEXT NOT NULL,
    type VARCHAR(50),
    read_at TIMESTAMPTZ,
    related_application_id UUID REFERENCES applications(application_id) ON DELETE SET NULL,
    related_session_id UUID REFERENCES mentorship_sessions(session_id) ON DELETE SET NULL,
    created_at TIMESTAMPTZ DEFAULT (NOW() AT TIME ZONE 'UTC')
);

CREATE TABLE audit_logs (
    log_id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    user_id UUID REFERENCES users(user_id) ON DELETE SET NULL,
    action VARCHAR(100) NOT NULL,
    table_name VARCHAR(100),
    record_id UUID,
    old_values JSONB,
    new_values JSONB,
    ip_address INET,
    user_agent TEXT,
    created_at TIMESTAMPTZ DEFAULT (NOW() AT TIME ZONE 'UTC')
);

-- Step 10: Indexes (Original + New)
CREATE INDEX idx_users_email ON users(email);
CREATE INDEX idx_users_role ON users(role);
CREATE INDEX idx_student_profiles_user_id ON student_profiles(user_id);
CREATE INDEX idx_educational_records_user_id ON educational_records(user_id);
CREATE INDEX idx_test_scores_user_id ON test_scores(user_id);
CREATE INDEX idx_test_scores_year_type ON test_scores(year, test_type);
CREATE INDEX idx_programs_department_id ON programs(department_id);
CREATE INDEX idx_departments_university_id ON departments(university_id);
CREATE INDEX idx_applications_user_id ON applications(user_id);
CREATE INDEX idx_applications_status ON applications(application_status);
CREATE INDEX idx_applications_university_program ON applications(university_id, program_id);
CREATE INDEX idx_documents_user_id ON documents(user_id);
CREATE INDEX idx_documents_application_id ON documents(application_id);
CREATE INDEX idx_mentorship_sessions_mentor_id ON mentorship_sessions(mentor_id);
CREATE INDEX idx_mentorship_sessions_student_id ON mentorship_sessions(student_id);
CREATE INDEX idx_notifications_user_id ON notifications(user_id);
-- (Indexes for new tables are created with the tables in Step 8)

-- Step 11: Triggers (Updated - removed student_profiles, mentors, university_representatives)
CREATE OR REPLACE FUNCTION update_updated_at_column()
RETURNS TRIGGER AS $$
BEGIN
    NEW.updated_at = (NOW() AT TIME ZONE 'UTC');
    RETURN NEW;
END;
$$ language 'plpgsql';

CREATE TRIGGER update_users_updated_at BEFORE UPDATE ON users
    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();
CREATE TRIGGER update_educational_records_updated_at BEFORE UPDATE ON educational_records
    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();
CREATE TRIGGER update_test_scores_updated_at BEFORE UPDATE ON test_scores
    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();
CREATE TRIGGER update_applications_updated_at BEFORE UPDATE ON applications
    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();
CREATE TRIGGER update_mentorship_sessions_updated_at BEFORE UPDATE ON mentorship_sessions
    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();
CREATE TRIGGER update_universities_updated_at BEFORE UPDATE ON universities
    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();
CREATE TRIGGER update_departments_updated_at BEFORE UPDATE ON departments
    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();
CREATE TRIGGER update_admission_cycles_updated_at BEFORE UPDATE ON admission_cycles
    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();
CREATE TRIGGER update_scholarships_updated_at BEFORE UPDATE ON scholarships
    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();
CREATE TRIGGER update_programs_updated_at BEFORE UPDATE ON programs
    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();
CREATE TRIGGER update_merit_formulas_updated_at BEFORE UPDATE ON merit_formulas
    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();
CREATE TRIGGER update_documents_updated_at BEFORE UPDATE ON documents
    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

-- Step 12: Sample Data (Original)
INSERT INTO universities (name, short_name, type, city, province, website_url, ranking_national) VALUES
('National University of Sciences and Technology', 'NUST', 'public', 'Islamabad', 'ICT', 'https://nust.edu.pk', 1),
('Lahore University of Management Sciences', 'LUMS', 'private', 'Lahore', 'Punjab', 'https://lums.edu.pk', 2),
('University of Engineering and Technology', 'UET Lahore', 'public', 'Lahore', 'Punjab', 'https://uet.edu.pk', 3);

INSERT INTO departments (university_id, name, short_name) 
SELECT university_id, 'School of Electrical Engineering and Computer Science', 'SEECS' 
FROM universities WHERE short_name = 'NUST';

INSERT INTO programs (department_id, name, degree_type, duration_years) 
SELECT department_id, 'Computer Science', 'BS', 4 
FROM departments WHERE short_name = 'SEECS';

INSERT INTO programs (department_id, name, degree_type, duration_years) 
SELECT department_id, 'Software Engineering', 'BS', 4 
FROM departments WHERE short_name = 'SEECS';

INSERT INTO merit_formulas (university_id, name, test_weightage, intermediate_weightage, matric_weightage, required_test_types, academic_year)
SELECT university_id, 'NUST Standard Formula', 75.00, 15.00, 10.00, ARRAY['NET'::test_type], '2024-25'
FROM universities WHERE short_name = 'NUST';

-- Final
COMMENT ON DATABASE oneuni IS 'OneUni - University Management System Database';