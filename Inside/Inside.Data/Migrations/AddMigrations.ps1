cd ..
Write-Host Migrating database changes...
$Date = Get-Date -format "yyyyMMddss"
$Text = '_Migration'
Write-Host $Date Migrations
dotnet ef migrations add $Date$Text
dotnet ef database update
Write-Host On any error try regenerating the database.