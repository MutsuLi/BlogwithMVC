#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
RUN echo "deb https://mirrors.tuna.tsinghua.edu.cn/debian/ buster main contrib non-free" > /etc/apt/sources.list
RUN apt-get update -y && apt-get install -y libgdiplus lsof && apt-get install -y libc6-dev
RUN ln -s /usr/lib/libgdiplus.so /usr/lib/gdiplus.dll
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Blog.MyBlog/Blog.App.csproj", "Blog.MyBlog/"]
COPY ["Blog.Data/Context.csproj", "Blog.Data/"]
COPY ["Blog.DataModels/Models.csproj", "Blog.DataModels/"]
COPY ["Blog.Login/Login.csproj", "Blog.Login/"]
COPY ["Blog.Utils/Utils.csproj", "Blog.Utils/"]
COPY ["Blog.Common/Common.csproj", "Blog.Common/"]
COPY ["Blog.Func/Func.csproj", "Blog.Func/"]



RUN dotnet restore "Blog.MyBlog/Blog.App.csproj"
COPY . .
WORKDIR "/src/Blog.MyBlog"
RUN dotnet build "Blog.App.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Blog.App.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Blog.Web.dll"]