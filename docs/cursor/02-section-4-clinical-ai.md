Refreshed 2026-11-15 from origin/master @ 6fc2554. Previous version dated 2026-03-04 (see git history / backup).
Verified against origin/master @ 6fc2554, 2026-11-15. The old file (320 lines) was replaced by this shorter file.

# Section 4: Clinical Documentation and AI

## 1. Scope [UPDATED]

Scrips.Core holds the AI wire contracts: chief-complaint DTOs, an Azure AI Search index class, and the AI rpcs on `MasterManagement`. It holds no model call, no prompt and no retrieval code. Scrips.Master owns the pipeline; Scrips.VideoApplication is a caller (per proto comments). [CONFIRMED: comments in `MasterManagement.proto:84-88, 166-169`]
Every clinical statement here is a quotation of a contract comment, not a clinical ruling. [NEEDS HUMAN REVIEW]
Any AI feature that touches Abu Dhabi patient data is also covered by the DoH AI compliance rules; none of that is decided in this repo. [NEEDS HUMAN REVIEW]

## 2. Files [UPDATED]

| Kind | Count | Location |
|---|---|---|
| DTOs | 16 | `Scrips.Core/Models/AIChiefComplaint/*.cs` |
| Refit routes | 4 AI routes on `IMasterApi` | `IMasterApi.cs:17, 19, 21, 23` (`POST /api/AISearch/RAGSearch`, `POST /api/AISearch/ClinicalSuggestions`, `POST /api/AISearch/GenerateDocumentation`, `GET /api/AISearch/ByNameAndCategory`) |
| gRPC rpcs on `MasterManagement` | 7 AI and vocabulary rpcs, all new since March | `Scrips.Core/Protos/MasterManagement.proto:84-106` |

DTO files: AnalyzedComplaint, ChiefComplaintDocument, ChiefComplaintDto, ClinicalSuggestionsRequest, DocumentationRequest, DocumentationResponse, HealthcareAnalysis, HealthcareEntity, HealthcareEntityAttribute, HealthcareEntityCategories, HealthcareEntityLink, HealthcareEntityRelation, RAGSearchRequest, RAGSearchResult, RAGSuggestion, TemporalContext. [CONFIRMED]

## 3. Azure AI Search index class [UPDATED]

`ChiefComplaintDocument` carries `Id` (key), `Name`, `Categories`, `ICD10Codes` (all `SearchableField`, analyzer `standard.lucene`) and `Embedding` (`float[]`, 1536 dimensions, profile `cc-vector-profile`). [CONFIRMED: `ChiefComplaintDocument.cs:8-40`] The index and profile are created elsewhere. [INFERRED]

## 4. gRPC AI rpcs on `MasterManagement` [NEW]

| rpc | Purpose (from the proto comment) | Evidence |
|---|---|---|
| `GenerateEmbedding` | Thin wrapper over Master's existing RAG and vector service methods, so ambient scribe reuses the pipeline | `MasterManagement.proto:84-89` |
| `SearchAndEnhance` | Similar cases plus guideline snippets; `topK` 1 to 20, default 5, server clamps silently | `MasterManagement.proto:90, 160-170` |
| `GetClinicalSuggestions` | Key questions, differentials, documentation elements, alerts | `MasterManagement.proto:91, 206-226` |
| `GenerateDocumentation` | Ambient-scribe SOAP draft plus structured findings (`rosFindings`, `examFindings`, `riskFindings`) | `MasterManagement.proto:92, 329-370` |
| `GetDrugNameVocabulary`, `GetDiagnosisVocabulary`, `GetOrderVocabulary` | Read-only vocabularies for the ambient scribe to correct misheard terms; `knownVersion` equal to the current version returns `notModified` | `MasterManagement.proto:93-106` (PROD-3195) |

### Explainability and review flag [NEW]
- `AIExplainability` has `reasoning`, `dataSources` (FHIR-style references) and `guidelineReference` (DHA, DOH, NICE, ADA, ACC or empty). [CONFIRMED: `MasterManagement.proto:139-146`]
- `clinicianReviewRequired` is `optional bool`. The comments say an unset value must be read as "review required". For the ambient SOAP the comment says it is "always true". [CONFIRMED: `MasterManagement.proto:183-184, 225-226, 365-370`; commit 6fc2554, PROD-623, 2026-11-14] Whether Scrips.Master sets it that way is not verified here. [INFERRED]
- `ErrorDetail` (field number 100 on responses) models partial failure: data and an error can come together. Codes named in the comment: PARTIAL_FAILURE, RAG_TIMEOUT, PARSER_FALLBACK, EMPTY_RESULT, UPSTREAM_ERROR. [CONFIRMED: comment at `MasterManagement.proto:108-121`]

### Wire values [NEW]
`Scrips.Core/Shared/Ambient/AmbientWireValues.cs` lists the exact strings the ambient encounter contract carries: `CodeStatus` (matched, ambiguous, unmatched, unchecked, empty), `ProcedureStatus` (performed, ordered), `Urgency` (routine, urgent, stat, empty), `Speaker` (provider, patient, other), `Assertion` (present, absent, normal, empty). Comments say: an empty assertion is "not stated, never read as absent or normal"; `unchecked` is to be treated like `unmatched`. [CONFIRMED: `AmbientWireValues.cs:14-72`] The file says Scrips.Master keeps a mirror pinned by a test and that the React client reads these strings. [CONFIRMED: comment `AmbientWireValues.cs:3-8`; the mirror and test are not verified]

## 5. Gaps and risks [NEW]

- `SearchAndEnhance` and `ClinicalSuggestions` requests carry no organization, patient or encounter id. The comment says the ambient consumer is `[Authorize]`-bypassed (CRIT-002), so there is "no honest source" for them yet. [CONFIRMED: comment `MasterManagement.proto:166-174, 206-211`; current status of CRIT-002 not verified] [NEEDS HUMAN REVIEW]
- The Refit AI routes (`IMasterApi`) and the gRPC AI rpcs are two parallel contracts for similar functions. Which one each consumer uses was not traced. [NOT VERIFIED]
- Old text said an AI chief-complaint feature depends on Azure Cognitive Search at v11.6.0. The package reference is unchanged (`Scrips.Core.csproj:11`). [CONFIRMED]
