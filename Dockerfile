# Stage 1: Build the application
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
# Inside the container, the current working directory is /src.

# Copy the .csproj file and restore dependencies first for caching efficiency
COPY ["CashbackTicket/CashbackTicket.csproj", "CashbackTicket/"]
# Explanation:
# The first part "CashbackTicket/CashbackTicket.csproj" is the SOURCE path on your host machine,
# relative to your build context (D:\CashbackTicket). So, it correctly points to D:\CashbackTicket\CashbackTicket\CashbackTicket.csproj.
# The second part "CashbackTicket/" is the DESTINATION path inside the container,
# relative to the current WORKDIR (/src). So, it copies to /src/CashbackTicket/CashbackTicket.csproj.
RUN dotnet restore "CashbackTicket/CashbackTicket.csproj"
# This command runs from /src and references /src/CashbackTicket/CashbackTicket.csproj, which is correct.

# Copy the rest of the application code
COPY . .
# Explanation:
# This copies EVERYTHING from your build context (D:\CashbackTicket) into the current WORKDIR (/src) inside the container.
# So, /src in the container will now contain:
#   /src/.git/
#   /src/CashbackTicket/ (this subfolder contains your source code, as copied from D:\CashbackTicket\CashbackTicket\)
#   /src/CashbackTicket.sln
#   etc.

WORKDIR /src/CashbackTicket
# Explanation:
# We now change the working directory inside the container to /src/CashbackTicket. This is where your actual C# project files reside within the container.

# Publish the application
RUN dotnet publish "CashbackTicket.csproj" -c Release -o /app/publish
# Explanation:
# This command runs from /src/CashbackTicket (due to the previous WORKDIR).
# "CashbackTicket.csproj" now correctly refers to /src/CashbackTicket/CashbackTicket.csproj.
# The output is directed to /app/publish (a new folder at the root of the container's file system).

# Stage 2: Create the final runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
# Explanation:
# The current working directory is /app.

COPY --from=build /app/publish .
# Explanation:
# Copies the published application binaries from the /app/publish folder in the 'build' stage
# to the current WORKDIR (/app) in the 'final' stage. So, /app will contain all your application DLLs.

EXPOSE 80
ENTRYPOINT ["dotnet", "CashbackTicket.dll"]
# Explanation:
# Executes the main DLL from the /app directory, which is correct.