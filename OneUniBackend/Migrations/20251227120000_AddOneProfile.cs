using System;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OneUniBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddOneProfile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_disabled",
                table: "student_profiles");

            migrationBuilder.AddColumn<string>(
                name: "father_name",
                table: "student_profiles",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "guardian_address",
                table: "student_profiles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "guardian_city",
                table: "student_profiles",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "guardian_cnic",
                table: "student_profiles",
                type: "character varying(15)",
                maxLength: 15,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "guardian_income",
                table: "student_profiles",
                type: "numeric(12,2)",
                precision: 12,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "guardian_phone",
                table: "student_profiles",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_orphan",
                table: "student_profiles",
                type: "boolean",
                nullable: true,
                defaultValue: false);

            migrationBuilder.AddColumn<JsonDocument>(
                name: "disability",
                table: "student_profiles",
                type: "jsonb",
                nullable: true);

            migrationBuilder.Sql("ALTER TABLE student_profiles ALTER COLUMN sports TYPE jsonb USING to_jsonb(sports);");

            migrationBuilder.AddColumn<Guid>(
                name: "educational_record_id",
                table: "documents",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "bucket",
                table: "documents",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "checksum",
                table: "documents",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "object_key",
                table: "documents",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "storage_provider",
                table: "documents",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<JsonDocument>(
                name: "metadata",
                table: "documents",
                type: "jsonb",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "idx_documents_educational_record_id",
                table: "documents",
                column: "educational_record_id");

            migrationBuilder.CreateIndex(
                name: "idx_documents_record_type_unique",
                table: "documents",
                columns: new[] { "educational_record_id", "document_type" },
                unique: true,
                filter: "educational_record_id IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "documents_educational_record_id_fkey",
                table: "documents",
                column: "educational_record_id",
                principalTable: "educational_records",
                principalColumn: "record_id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "documents_educational_record_id_fkey",
                table: "documents");

            migrationBuilder.DropIndex(
                name: "idx_documents_educational_record_id",
                table: "documents");

            migrationBuilder.DropIndex(
                name: "idx_documents_record_type_unique",
                table: "documents");

            migrationBuilder.DropColumn(
                name: "educational_record_id",
                table: "documents");

            migrationBuilder.DropColumn(
                name: "bucket",
                table: "documents");

            migrationBuilder.DropColumn(
                name: "checksum",
                table: "documents");

            migrationBuilder.DropColumn(
                name: "object_key",
                table: "documents");

            migrationBuilder.DropColumn(
                name: "storage_provider",
                table: "documents");

            migrationBuilder.DropColumn(
                name: "metadata",
                table: "documents");

            migrationBuilder.DropColumn(
                name: "father_name",
                table: "student_profiles");

            migrationBuilder.DropColumn(
                name: "guardian_address",
                table: "student_profiles");

            migrationBuilder.DropColumn(
                name: "guardian_city",
                table: "student_profiles");

            migrationBuilder.DropColumn(
                name: "guardian_cnic",
                table: "student_profiles");

            migrationBuilder.DropColumn(
                name: "guardian_income",
                table: "student_profiles");

            migrationBuilder.DropColumn(
                name: "guardian_phone",
                table: "student_profiles");

            migrationBuilder.DropColumn(
                name: "is_orphan",
                table: "student_profiles");

            migrationBuilder.DropColumn(
                name: "disability",
                table: "student_profiles");

            migrationBuilder.Sql("ALTER TABLE student_profiles ALTER COLUMN sports TYPE character varying(100) USING sports::text;");

            migrationBuilder.AddColumn<bool>(
                name: "is_disabled",
                table: "student_profiles",
                type: "boolean",
                nullable: true,
                defaultValue: false);
        }
    }
}

