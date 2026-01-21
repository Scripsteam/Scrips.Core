# Master Data & Reference Systems - Complete Documentation

## OVERVIEW

**Purpose:** Provide lookup values (gender, marital status, countries) and medical terminology references (ICD-10, SNOMED CT, CPT)

**Key Entities:** IdNamePair (lookup values), Speciality (medical specialties), PracticeValueSet (terminology)

**Key Workflows:** Lookup value retrieval, medical code validation, terminology reference

**PHI Scope:** NO - Reference data only (no patient-specific information)

---

## ENTITIES

### IdNamePair

**Location:** Models/IdNamePair.cs

**Purpose:** Generic key-value pair for lookup values

**Fields:**
- Id (Guid or string) - Lookup identifier
- Name (string) - Display value

**Usage:** Gender, MaritalStatus, IdentityType, OwnerType, Countries, Languages

---

### Speciality

**Location:** Models/Scheduling/Speciality.cs:18

**Purpose:** Medical specialties with SNOMED CT code references

**Key Fields:**
- SpecialityId
- SpecialityName
- SNOMEDCode - SNOMED CT reference
- System - "https://snomed.info/sct" (line 18)

---

### PracticeValueSet

**Location:** Referenced with SNOMED CT system property

**Purpose:** Clinical terminology value sets for standardization

**System:** https://snomed.info/sct (PracticeValueSet.cs:23)

---

## WORKFLOWS

### Workflow 1: Lookup Value Retrieval

**Entry Point:** IMasterApi.cs:9-15

**Endpoints:**
- GET /api/Master/Gender (line 9)
- GET /api/Master/IdentityType (line 11)
- GET /api/Master/MaritalStatus (line 13)
- GET /api/Master/OwnerType (line 15)

**Steps:**
1. **Request** - GET lookup endpoint
2. **Cache Lookup** - Check cache (NOT IMPLEMENTED - ICacheService.cs:1-16)
3. **Database Query** - If not cached
4. **Return List\<IdNamePair\>** - For dropdown population

**Error Handling:**
- Lookup service down: Return cached values (cache not implemented)
- Database timeout: Retry or return empty list

**Performance:** Should be cached to reduce database load (NOT IMPLEMENTED)

---

### Workflow 2: Medical Code Validation

**Entry Point:** Referenced in clinical workflows (ICD-10, CPT, SNOMED)

**Standards:**
- **ICD-10** - Diagnosis codes (10th revision)
- **CPT** - Current Procedural Terminology (procedure codes)
- **HCPCS** - Healthcare Common Procedure Coding System
- **SNOMED CT** - Systematized Nomenclature of Medicine - Clinical Terms

**Validation:** Verify code exists in standard terminology

---

## INTEGRATIONS

**Master Data Service (IMasterApi.cs:7-25):**
- 4 lookup endpoints: Gender, IdentityType, MaritalStatus, OwnerType
- 4 AI endpoints: RAGSearch, ClinicalSuggestions, GenerateDocumentation, ByNameAndCategory

**SNOMED CT:**
- Reference: https://snomed.info/sct
- Purpose: Medical terminology standardization
- Usage: Specialty codes (Speciality.cs:18), value sets (PracticeValueSet.cs:23)

---

## COMPLIANCE SUMMARY

**PHI:** NONE (reference data only)

**Audit:** Minimal (reference data access not typically audited)

**Standards:**
- **ICD-10** (diagnoses) - Required for billing and claims
- **SNOMED CT** (clinical findings) - Required for NABIDH health information exchange
- **CPT/HCPCS** (procedures) - Required for billing

**Caching:** SHOULD be implemented (currently NOT IMPLEMENTED - ICacheService.cs:1-16)

---

## CRITICAL FINDINGS

**Risk #1: No Distributed Cache (MEDIUM)**
- **Issue:** ICacheService interface only, no implementation
- **Impact:** Repeated database queries for lookup data
- **Evidence:** Technical Debt #6 (Performance Debt #2)
- **Mitigation:** Implement Redis cache with encryption (40 hours effort)

**Risk #2: Lookup Service Single Point of Failure (LOW)**
- **Issue:** No caching means lookup failures break forms
- **Impact:** Cannot populate dropdowns (gender, marital status)
- **Mitigation:** Implement cache with graceful degradation

---

**Document Version:** 1.0  
**Last Updated:** January 21, 2026  
**Audited Against:** Scrips.Core v7.0 (.NET 7.0)
