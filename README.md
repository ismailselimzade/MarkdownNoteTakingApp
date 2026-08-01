# Markdown Note-taking App

A REST API for creating and managing markdown notes with file attachments, live markdown rendering, and grammar checking.  
Fayl əlavələri, canlı markdown render və qrammatika yoxlaması ilə markdown qeydləri yaratmaq və idarə etmək üçün REST API.

---

## Features / Xüsusiyyətlər

- User authentication with JWT — register, login, update profile, delete account.  
  JWT ilə istifadəçi autentifikasiyası — qeydiyyat, giriş, profil yeniləmə, hesab silmə.

- Full CRUD support for notes, with partial updates (title-only or content-only).  
  Qeydlər üçün tam CRUD dəstəyi, qismən yeniləmə imkanı ilə (yalnız başlıq və ya yalnız məzmun).

- Renders note content from markdown to HTML using Markdig.  
  Markdig vasitəsilə qeyd məzmununu markdown-dan HTML-ə çevirir.

- Grammar checking for note content via the LanguageTool API.  
  LanguageTool API vasitəsilə qeyd məzmununun qrammatika yoxlaması.

- File attachments per note — upload, list, download, and delete, stored on disk.  
  Hər qeyd üçün fayl əlavələri — yükləmə, siyahılama, endirmə və silmə, diskdə saxlanılır.

- Each user can only access their own notes and attachments.  
  Hər istifadəçi yalnız öz qeydlərinə və əlavələrinə daxil ola bilər.

- Interactive API documentation via Scalar.  
  Scalar vasitəsilə interaktiv API sənədləşməsi.

---

## What I Learned / Öyrəndiklərim

- How to integrate a third-party markdown renderer (Markdig) into an API response.  
  Üçüncü tərəf markdown render kitabxanasını (Markdig) API cavabına necə inteqrasiya etmək olar.

- How to call an external grammar-checking API and map its response to custom DTOs.  
  Xarici qrammatika yoxlama API-sini çağırıb cavabını öz DTO-larına necə uyğunlaşdırmaq olar.

- How to handle file uploads and downloads with IFormFile, storing files on disk and metadata in the database.  
  IFormFile ilə fayl yükləmə və endirməni necə idarə etmək olar, faylları diskdə, metadata-nı isə bazada saxlamaqla.

- How mismatched route parameter names can cause silent bugs or route collisions between actions.  
  Uyğun olmayan route parametr adlarının action-lar arasında sükutlu bug-lara və ya route toqquşmalarına necə səbəb ola biləcəyini.

- How nullable reference types affect ASP.NET Core's automatic model validation for partial updates.  
  Nullable reference type-ların ASP.NET Core-un avtomatik model validasiyasına qismən yeniləmələr üçün necə təsir etdiyini.

- How to set up Scalar as a modern alternative to Swagger UI for exploring and testing the API.  
  API-ni araşdırmaq və test etmək üçün Scalar-ı Swagger UI-ın müasir alternativi kimi necə qurmaq olar.

---

## Tech Stack / Texnologiyalar

C#, ASP.NET Core, Entity Framework Core, SQL Server, JWT, BCrypt.Net, Markdig, LanguageTool API, Scalar

---

## Endpoints

### Auth

| Method | URL | Description |
|--------|-----|-------------|
| POST | `/api/auth/register` | Registers a new user. / Yeni istifadəçi qeydiyyatı. |
| POST | `/api/auth/login` | Returns a JWT token. / JWT token qaytarır. |
| GET | `/api/auth/profile` | Returns the current user's profile. / Cari istifadəçinin profilini qaytarır. |
| PUT | `/api/auth/update` | Updates the current user's username or password. / Cari istifadəçinin istifadəçi adını və ya şifrəsini yeniləyir. |
| DELETE | `/api/auth/delete` | Deletes the current user's account. / Cari istifadəçinin hesabını silir. |

### Notes

| Method | URL | Description |
|--------|-----|-------------|
| GET | `/api/notes` | Returns all notes of the current user. / Cari istifadəçinin bütün qeydlərini qaytarır. |
| GET | `/api/notes/{id}` | Returns a specific note. / Müəyyən qeydi qaytarır. |
| POST | `/api/notes` | Creates a new note. / Yeni qeyd yaradır. |
| PUT | `/api/notes/{id}` | Updates a note (title and/or content). / Qeydi yeniləyir (başlıq və/və ya məzmun). |
| DELETE | `/api/notes/{id}` | Deletes a note. / Qeydi silir. |
| GET | `/api/notes/render/{id}` | Returns the note content rendered as HTML. / Qeyd məzmununu HTML kimi render edilmiş şəkildə qaytarır. |
| GET | `/api/notes/grammar-check/{id}` | Returns grammar check results for the note content. / Qeyd məzmunu üçün qrammatika yoxlama nəticələrini qaytarır. |

### Attachments

| Method | URL | Description |
|--------|-----|-------------|
| POST | `/api/attachments` | Uploads a file attachment for a note. / Qeyd üçün fayl əlavəsi yükləyir. |
| GET | `/api/attachments/note/{noteId}` | Returns all attachments for a note. / Qeydin bütün əlavələrini qaytarır. |
| GET | `/api/attachments/{id}` | Downloads a specific attachment. / Müəyyən əlavəni endirir. |
| DELETE | `/api/attachments/{id}` | Deletes a specific attachment. / Müəyyən əlavəni silir. |

---

## Setup / Quraşdırma

1. Clone the repository.  
   Repozitorini kopyalayın.

2. Add the connection string and JWT settings to `appsettings.json`.  
   `appsettings.json` faylına connection string və JWT parametrlərini əlavə edin.

3. Apply the database migrations.  
   Veritabanı miqrasiyalarını tətbiq edin.

    `dotnet ef database update`

4. Run the project.  
   Proyekti işə salın.

   `dotnet run`

5. Open the interactive API docs.  
   İnteraktiv API sənədlərini açın.

   `https://localhost:7171/scalar/v1`
