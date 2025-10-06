-- SQL Migration Script: Add Missing Enum Columns
-- This script adds enum columns that were missing from the scaffolded models
-- Run this script against your PostgreSQL database

BEGIN;

-- Add role column to users table
ALTER TABLE users 
ADD COLUMN IF NOT EXISTS role user_role NOT NULL DEFAULT 'student';

-- Add status column to applications table
ALTER TABLE applications 
ADD COLUMN IF NOT EXISTS status application_status NOT NULL DEFAULT 'draft';

-- Add gender, id_document_type, and guardian_relation columns to student_profiles table
ALTER TABLE student_profiles 
ADD COLUMN IF NOT EXISTS gender gender_type,
ADD COLUMN IF NOT EXISTS id_document_type id_document_type,
ADD COLUMN IF NOT EXISTS guardian_relation guardian_relation;

-- Add document_type and verification_status columns to documents table
ALTER TABLE documents 
ADD COLUMN IF NOT EXISTS document_type document_type NOT NULL DEFAULT 'other',
ADD COLUMN IF NOT EXISTS verification_status verification_status NOT NULL DEFAULT 'pending';

-- Add education_type column to educational_records table
ALTER TABLE educational_records 
ADD COLUMN IF NOT EXISTS education_type education_type NOT NULL DEFAULT 'matric';

-- Add test_type column to test_scores table
ALTER TABLE test_scores 
ADD COLUMN IF NOT EXISTS test_type test_type NOT NULL DEFAULT 'other';

-- Add session_type and session_status columns to mentorship_sessions table
ALTER TABLE mentorship_sessions 
ADD COLUMN IF NOT EXISTS session_type session_type NOT NULL DEFAULT 'free',
ADD COLUMN IF NOT EXISTS session_status session_status NOT NULL DEFAULT 'scheduled';

COMMIT;

-- Verification queries to check if columns were added successfully
SELECT 
    'users' as table_name,
    column_name,
    data_type,
    udt_name
FROM information_schema.columns 
WHERE table_name = 'users' AND column_name = 'role'
UNION ALL
SELECT 
    'applications' as table_name,
    column_name,
    data_type,
    udt_name
FROM information_schema.columns 
WHERE table_name = 'applications' AND column_name = 'status'
UNION ALL
SELECT 
    'student_profiles' as table_name,
    column_name,
    data_type,
    udt_name
FROM information_schema.columns 
WHERE table_name = 'student_profiles' AND column_name IN ('gender', 'id_document_type', 'guardian_relation')
UNION ALL
SELECT 
    'documents' as table_name,
    column_name,
    data_type,
    udt_name
FROM information_schema.columns 
WHERE table_name = 'documents' AND column_name IN ('document_type', 'verification_status')
UNION ALL
SELECT 
    'educational_records' as table_name,
    column_name,
    data_type,
    udt_name
FROM information_schema.columns 
WHERE table_name = 'educational_records' AND column_name = 'education_type'
UNION ALL
SELECT 
    'test_scores' as table_name,
    column_name,
    data_type,
    udt_name
FROM information_schema.columns 
WHERE table_name = 'test_scores' AND column_name = 'test_type'
UNION ALL
SELECT 
    'mentorship_sessions' as table_name,
    column_name,
    data_type,
    udt_name
FROM information_schema.columns 
WHERE table_name = 'mentorship_sessions' AND column_name IN ('session_type', 'session_status')
ORDER BY table_name, column_name;

