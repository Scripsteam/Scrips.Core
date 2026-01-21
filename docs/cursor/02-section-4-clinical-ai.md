# Clinical Documentation & AI Decision Support - Complete Documentation

## OVERVIEW

**Purpose:** AI-powered chief complaint analysis, ICD-10 code suggestions, clinical documentation generation via RAG (Retrieval Augmented Generation)

**Key Entities:** ChiefComplaintDocument, RAGSearchResult, HealthcareEntity, ClinicalSuggestionsRequest, DocumentationResponse

**Key Workflows:** Chief complaint NLP analysis, vector similarity search, AI-assisted ICD-10 coding, clinical note generation

**PHI Scope:** YES - Chief complaint text, symptoms, clinical context (clinical PHI)

---

## ENTITIES

### ChiefComplaintDocument

**Location:** Models/AIChiefComplaint/ChiefComplaintDocument.cs:6

**Purpose:** Azure Cognitive Search indexed document with OpenAI vector embeddings for semantic search

**Fields:**

| Field | Type | Purpose | PHI? |
|-------|------|---------|------|
| Id | string | Unique document ID | NO |
| Name | string | Chief complaint text | **YES** |
| Categories | IList\<string\> | Healthcare categories | **YES** |
| ICD10Codes | IList\<string\> | Associated diagnosis codes | **YES** |
| Embedding | float[] | 1536-dimension vector (OpenAI text-embedding-ada-002) | **YES** |

**Azure Search Configuration:**
- SearchableField: Name, Categories, ICD10Codes (full-text search)
- VectorSearchField: Embedding with 1536 dimensions
- VectorSearchProfileName: "cc-vector-profile"
- Analyzer: standard.lucene

**File:** ChiefComplaintDocument.cs:8,12,19,26,32

---

### HealthcareEntity

**Location:** Models/AIChiefComplaint/HealthcareEntity.cs

**Purpose:** NLP-extracted medical entities (symptoms, conditions, anatomical sites, medications)

**Key Concepts:**
- Entity extraction from free-text chief complaints
- Categories: Symptom, Condition, BodyPart, Medication, Severity, TemporalContext
- Relationships: HealthcareEntityRelation links entities

---

### RAGSearchResult / RAGSuggestion

**Location:** Models/AIChiefComplaint/RAGSearchResult.cs, RAGSuggestion.cs

**Purpose:** Similar case retrieval with ICD-10 code suggestions and confidence scores

**RAG Pattern:** Retrieval (find similar cases) → Augmentation (add context) → Generation (suggest codes/documentation)

---

### DocumentationResponse

**Location:** Models/AIChiefComplaint/DocumentationResponse.cs

**Purpose:** AI-generated clinical assessment and plan based on similar cases

**Workflow:** Chief complaint + RAG results → GPT-4 generates structured note → Provider reviews/edits

---

## WORKFLOWS

### Workflow 1: Chief Complaint NLP Analysis

**Entry Point:** IMasterApi.cs:17 (RAGSearch), 19 (ClinicalSuggestions)

**Steps:**
1. **Text Input** - Provider/patient enters chief complaint
2. **Entity Extraction** - NLP identifies symptoms, body parts, severity
   - HealthcareEntity extraction
3. **Healthcare Categorization** - Classify by system (respiratory, cardiovascular, etc.)
4. **Vector Embedding** - OpenAI text-embedding-ada-002 (1536 dimensions)
5. **Azure Cognitive Search** - Vector similarity search
6. **RAG Search Results** - Top K similar cases with ICD-10 codes
7. **ICD-10 Suggestions** - Ranked by relevance with confidence
8. **Provider Review** - Accept, modify, or reject suggestions

**Files:**
- IMasterApi.cs:17 (POST /api/AISearch/RAGSearch)
- IMasterApi.cs:19 (POST /api/AISearch/ClinicalSuggestions)
- ChiefComplaintDocument.cs:32-36 (vector search field)

**Error Handling:**
- Azure Search unavailable: Fallback to keyword search
- Embedding API timeout: Retry or manual coding
- No similar cases found: Provider codes manually

**Healthcare Compliance:**
- **PHI:** Chief complaint text, symptoms (clinical PHI)
- **Audit:** ✅ AI suggestions logged for clinical decision trail
- **Clinical Safety:** AI suggestions require provider review before acceptance
- **Model Training:** Embeddings from de-identified data only

---

### Workflow 2: AI Clinical Documentation

**Entry Point:** IMasterApi.cs:21 (GenerateDocumentation)

**Steps:**
1. **Chief Complaint** - Input text
2. **RAG Search** - Retrieve similar cases
3. **AI Generation** - GPT-4 drafts assessment and plan
4. **Structured Output** - DocumentationResponse
5. **Provider Edit** - Review and finalize note
6. **Save to EHR** - Link to patient encounter

**File:** IMasterApi.cs:21 (POST /api/AISearch/GenerateDocumentation)

**Clinical Safety:** AI-generated notes are DRAFT only, require provider sign-off

---

### Workflow 3: Vector Similarity Search

**Technical Flow:**
1. **Text → Embedding** - OpenAI API converts text to 1536-dimension vector
2. **Index Query** - Azure Cognitive Search vector similarity
3. **Cosine Similarity** - Find nearest neighbors in embedding space
4. **Ranking** - Return top K results with relevance scores
5. **Hybrid Search** - Combine vector + keyword for best results

**Performance:** ~100-500ms for search (Azure API latency)

---

## INTEGRATIONS

**Master Data Service (IMasterApi.cs:7-25):**
- 4 AI endpoints: RAGSearch, ClinicalSuggestions, GenerateDocumentation, ByNameAndCategory
- Purpose: AI-powered clinical decision support

**Azure Cognitive Search:**
- Vector search with OpenAI embeddings
- ChiefComplaintDocument.cs:8 (Azure.Search.Documents v11.6.0)

**SNOMED CT:**
- Medical terminology reference
- URL: https://snomed.info/sct

---

## COMPLIANCE SUMMARY

**PHI Fields:** Chief complaint text, symptoms, clinical context (ALL clinical PHI)

**Audit:** ✅ AI-assisted suggestions logged (HIPAA 164.308 - clinical decision trail)

**Clinical Safety:** AI suggestions are recommendations only, require provider review

**Model Training:** Embeddings must be from de-identified training data

**Data Residency:** Azure region must comply with UAE data residency requirements

**Retention:** Clinical notes 10 years, AI training data de-identified

---

## CRITICAL FINDINGS

**Risk #1: No TTL on Azure Search Documents**
- Chief complaints indexed indefinitely
- Impact: PHI retention compliance violation
- Mitigation: Implement TTL policy on search index

**Risk #2: AI Model Bias**
- OpenAI embeddings trained on English medical literature
- Impact: May be less accurate for non-English or culturally specific complaints
- Mitigation: Validate model performance on local patient population

---

**Document Version:** 1.0  
**Last Updated:** January 21, 2026  
**Audited Against:** Scrips.Core v7.0 (.NET 7.0)
