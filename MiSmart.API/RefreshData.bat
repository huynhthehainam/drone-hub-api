﻿
GOOGLE_APPLICATION_CREDENTIALS=google_services.json dotnet ef database drop -f
GOOGLE_APPLICATION_CREDENTIALS=google_services.json dotnet ef migrations remove
GOOGLE_APPLICATION_CREDENTIALS=google_services.json dotnet ef migrations add Initial
GOOGLE_APPLICATION_CREDENTIALS=google_services.json dotnet ef database update
