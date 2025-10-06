# OneUni Backend - Database Setup Documentation

## Overview
This document explains the database setup for the OneUni backend application using ASP.NET Core with Entity Framework Core and PostgreSQL.

## Database Setup Completed

### 1. ✅ Fixed Connection String Configuration
- **Fixed:** Removed hardcoded connection string from `OneuniContext.cs`
- **Updated:** `appsettings.json` now contains the complete connection string
- **Location:** Connection string is now properly managed in `appsettings.json`
- **Current Configuration:**
  ```json
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=oneuni;Username=postgres;Password=postgres"
  }
  ```

### 2. ✅ Updated Enum Mapping to Modern Npgsql 7.0+ Approach
- **Fixed:** Replaced deprecated `NpgsqlConnection.GlobalTypeMapper` with modern data source builder pattern
- **Location:** `Program.cs`
- **Benefit:** Eliminates deprecation warnings and uses the recommended approach

### 3. ✅ Added Missing Enum Properties to Models

The following enum properties were added to ensure proper type safety and database schema alignment:

#### **User Model** (`Models/User.cs`)
- Added `UserRole Role` property

#### **Application Model** (`Models/Application.cs`)
- Added `ApplicationStatus Status` property

#### **StudentProfile Model** (`Models/StudentProfile.cs`)
- Added `GenderType? Gender` property
- Added `IdDocumentType? IdDocumentType` property
- Added `GuardianRelation? GuardianRelation` property

#### **Document Model** (`Models/Document.cs`)
- Added `DocumentType DocumentType` property
- Added `VerificationStatus VerificationStatus` property

#### **EducationalRecord Model** (`Models/EducationalRecord.cs`)
- Added `EducationType EducationType` property

#### **TestScore Model** (`Models/TestScore.cs`)
- Added `TestType TestType` property

#### **MentorshipSession Model** (`Models/MentorshipSession.cs`)
- Added `SessionType SessionType` property
- Added `SessionStatus SessionStatus` property

### 4. ✅ Updated Entity Configurations in OneuniContext
All new enum properties have been properly configured in the `OnModelCreating` method with correct column mappings.

## Database Migration Required

Since this project was scaffolded from an existing PostgreSQL database, the database schema needs to be updated with the new enum columns.

### Option 1: Using SQL Script (Recommended)

Run the provided SQL migration script:

```bash
# Navigate to the Migrations directory
cd Migrations

# Apply the migration using psql
psql -h localhost -p 5432 -U postgres -d oneuni -f AddMissingEnumColumns.sql
```

Or use any PostgreSQL client (pgAdmin, DBeaver, etc.) to execute the `AddMissingEnumColumns.sql` script.

### Option 2: Manual Database Update

If you prefer to update manually, run these SQL commands in your PostgreSQL database:

```sql
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
```

## PostgreSQL Enum Types

The following PostgreSQL enum types are already defined in your database:

- `user_role` - student, mentor, university_representative, admin
- `gender_type` - male, female, other, prefer_not_to_say
- `education_type` - matric, intermediate, a_levels, o_levels, equivalent
- `test_type` - NET, ECAT, MDCAT, SAT, IELTS, TOEFL, FAST, LUMS, other
- `application_status` - draft, scheduled, submitted, under_review, await_merit_list, awaiting_fee_submission, accepted, rejected
- `session_type` - free, paid
- `session_status` - scheduled, completed, cancelled, no_show
- `document_type` - matric_certificate, intermediate_certificate, transcript, cnic, passport, nicop, b_form, sports_certificate, hafiz_certificate, income_certificate, domicile, other
- `verification_status` - pending, verified, rejected
- `guardian_relation` - father, mother, guardian, other
- `id_document_type` - cnic, nicop, passport, b_form

## Environment Configuration

### Production Setup
For production, update `appsettings.json` with secure credentials:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=your-db-host;Port=5432;Database=oneuni;Username=your-username;Password=your-secure-password"
  }
}
```

### Development Setup
For development, you can use environment variables or user secrets:

```bash
# Using dotnet user-secrets
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=oneuni;Username=postgres;Password=your-password"
```

## Verification

### Build the Project
```bash
dotnet build
```
✅ Should build with **0 warnings** and **0 errors**

### Test Database Connection
```bash
dotnet run
```
You should see:
```
✅ Database connection successful!
```

### Verify Database Schema
Run the verification queries in the SQL script to confirm all columns exist:

```sql
SELECT 
    table_name,
    column_name,
    data_type,
    udt_name
FROM information_schema.columns 
WHERE 
    (table_name = 'users' AND column_name = 'role')
    OR (table_name = 'applications' AND column_name = 'status')
    OR (table_name = 'student_profiles' AND column_name IN ('gender', 'id_document_type', 'guardian_relation'))
    OR (table_name = 'documents' AND column_name IN ('document_type', 'verification_status'))
    OR (table_name = 'educational_records' AND column_name = 'education_type')
    OR (table_name = 'test_scores' AND column_name = 'test_type')
    OR (table_name = 'mentorship_sessions' AND column_name IN ('session_type', 'session_status'))
ORDER BY table_name, column_name;
```

## Project Structure

```
OneUniBackend/
├── Models/                    # Entity models
│   ├── Enum.cs               # All enum definitions
│   ├── User.cs               # User entity
│   ├── Application.cs        # Application entity
│   ├── StudentProfile.cs     # Student profile entity
│   ├── Document.cs           # Document entity
│   ├── EducationalRecord.cs  # Educational record entity
│   ├── TestScore.cs          # Test score entity
│   ├── MentorshipSession.cs  # Mentorship session entity
│   └── OneuniContext.cs      # EF Core DbContext
├── Migrations/               # Database migrations
│   └── AddMissingEnumColumns.sql
├── Controllers/              # API controllers (empty - ready for implementation)
├── Services/                 # Business logic services (empty)
├── Program.cs               # Application entry point
└── appsettings.json         # Configuration

```

## Next Steps

1. ✅ Database setup is complete
2. ⏭️ Apply the SQL migration script to add missing enum columns
3. ⏭️ Start building API controllers and services
4. ⏭️ Implement authentication and authorization
5. ⏭️ Add business logic and validation

## Troubleshooting

### Connection Failed
- Verify PostgreSQL is running: Check if the service is active
- Check connection string: Ensure host, port, database name, username, and password are correct
- Check firewall: Ensure port 5432 is accessible

### Enum Type Errors
- Verify enum types exist in PostgreSQL database
- Check that enum values in code match database definitions
- Ensure enum mapping is configured in `Program.cs`

### Build Warnings
- If you see `CS0618` warnings about `GlobalTypeMapper`, ensure you're using the modern data source builder pattern as shown in this setup

## Summary

✅ **Database Configuration:** Properly configured with secure connection management  
✅ **Enum Mapping:** Updated to modern Npgsql 7.0+ approach  
✅ **Model Properties:** All missing enum properties added  
✅ **Entity Configuration:** All enum properties properly mapped in DbContext  
✅ **Build Status:** Clean build with 0 warnings, 0 errors  
⏳ **Database Schema Update:** Ready to apply SQL migration script

The project is now properly configured and ready for feature development!

