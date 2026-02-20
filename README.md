<div align="center">

# 🎮 API-PUBG

<img src="https://readme-typing-svg.herokuapp.com?font=Fira+Code&weight=600&size=28&duration=4000&pause=1000&color=F75C03&center=true&vCenter=true&width=600&lines=PUBG+Data+API;ASP.NET+Core+Backend;Game+Statistics+%26+Analytics" alt="Typing SVG" />

[![GitHub stars](https://img.shields.io/github/stars/Yonderus/API-PUBG?style=for-the-badge&logo=github&color=yellow)](https://github.com/Yonderus/API-PUBG/stargazers)
[![GitHub forks](https://img.shields.io/github/forks/Yonderus/API-PUBG?style=for-the-badge&logo=github&color=blue)](https://github.com/Yonderus/API-PUBG/network)
[![GitHub issues](https://img.shields.io/github/issues/Yonderus/API-PUBG?style=for-the-badge&logo=github&color=red)](https://github.com/Yonderus/API-PUBG/issues)
[![License](https://img.shields.io/github/license/Yonderus/API-PUBG?style=for-the-badge)](LICENSE)

### 🚀 A powerful C# ASP.NET Core API for PUBG game data integration

[Características](#-características) •
[Instalación](#-instalación) •
[Uso](#-uso) •
[Arquitectura](#-arquitectura) •
[Contribuir](#-contribuir)

</div>

---

## 📖 Descripción

<div align="center">
<img src="https://capsule-render.vercel.app/api?type=waving&color=gradient&customColorList=12,14,18&height=100&section=header&animation=fadeIn" width="100%"/>
</div>

**API-PUBG** es un servicio backend robusto desarrollado con **C#** y **ASP.NET Core**, diseñado para servir y gestionar datos relacionados con PlayerUnknown's Battlegrounds (PUBG). Proporciona una forma estructurada de acceder a estadísticas de juego, perfiles de jugadores, datos de partidas y otra información relacionada con el juego.

Ideal para:
- 📊 Herramientas de análisis de datos
- 🎯 Aplicaciones companion
- 🌐 Interfaces web personalizadas
- 📈 Dashboards de estadísticas

---

## ✨ Características

<details open>
<summary><b>Ver todas las características</b></summary>
<br>

| Característica | Descripción |
|:---:|:---|
| 🎯 | **Integración PUBG**: Interacción fluida con datos del juego |
| 🌐 | **API RESTful**: Endpoints limpios y bien definidos |
| 💾 | **Modelos de Datos**: Estructuras para jugadores, partidas y estadísticas |
| ⚙️ | **Lógica de Servicios**: Capa de negocio encapsulada y mantenible |
| 🔄 | **Arquitectura Extensible**: Fácil expansión de funcionalidades |
| 🛡️ | **Type-Safe**: Aprovecha el tipado fuerte de C# |
| ⚡ | **Alto Rendimiento**: Optimizado con ASP.NET Core |
| 📦 | **Modular**: Separación clara de responsabilidades |

</details>

---

## 🛠️ Tech Stack

<div align="center">

![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white)
![.NET](https://img.shields.io/badge/.NET-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![NuGet](https://img.shields.io/badge/NuGet-004880?style=for-the-badge&logo=nuget&logoColor=white)

</div>

---

## 🚀 Instalación

### Prerrequisitos

```bash
✅ .NET SDK 8.0 o superior
✅ Visual Studio 2022 / VS Code / Rider
✅ Git
```

### Pasos de Instalación

<details open>
<summary><b>1️⃣ Clonar el repositorio</b></summary>

```bash
git clone https://github.com/Yonderus/API-PUBG.git
cd API-PUBG
```
</details>

<details>
<summary><b>2️⃣ Restaurar dependencias</b></summary>

```bash
dotnet restore
```
</details>

<details>
<summary><b>3️⃣ Configurar variables de entorno</b></summary>

```bash
# Crear archivo appsettings.json con tu configuración
cp appsettings.example.json appsettings.json
```
</details>

<details>
<summary><b>4️⃣ Compilar el proyecto</b></summary>

```bash
dotnet build
```
</details>

<details>
<summary><b>5️⃣ Ejecutar la aplicación</b></summary>

```bash
dotnet run
```
</details>

---

## 🎯 Uso

### Ejemplo de Petición API

```csharp
// GET: api/players/{playerName}
GET https://localhost:5001/api/players/YONDERUS
```

### Respuesta Esperada

```json
{
  "playerId": "12345",
  "playerName": "YONDERUS",
  "stats": {
    "kills": 150,
    "deaths": 75,
    "wins": 20,
    "kd_ratio": 2.0
  }
}
```

---

## 🏗️ Arquitectura

<div align="center">

```mermaid
graph TB
    A[👤 Cliente] -->|HTTP Request| B[🎮 PUBG-Controller]
    B --> C[⚙️ PUBG-Services]
    C --> D[💾 PUBG-Model]
    C --> E[🌐 API Externa PUBG]
    D --> F[(📊 Base de Datos)]
    B --> G[👁️ PUBG-Views]
    
    style A fill:#e1f5ff
    style B fill:#ffe1e1
    style C fill:#fff4e1
    style D fill:#e1ffe1
    style E fill:#f0e1ff
    style F fill:#ffe1f0
    style G fill:#e1ffff
```

</div>

### Estructura del Proyecto

```
API-PUBG/
├── 📁 PUBG-Controller/     # Controladores de API (Endpoints)
├── 📁 PUBG-Model/          # Modelos de datos y entidades
├── 📁 PUBG-Services/       # Lógica de negocio y servicios
├── 📁 PUBG-Views/          # Vistas y DTOs
├── 📄 API PUBG.sln         # Archivo de solución
└── 📄 README.md            # Este archivo
```

---

## 🤝 Contribuir

<div align="center">

Las contribuciones son **bienvenidas** y **apreciadas** 🎉

</div>

### Proceso de Contribución

1. 🍴 Fork el proyecto
2. 🌿 Crea tu Feature Branch (`git checkout -b feature/AmazingFeature`)
3. 💾 Commit tus cambios (`git commit -m 'Add: nueva característica increíble'`)
4. 📤 Push a la Branch (`git push origin feature/AmazingFeature`)
5. 🔀 Abre un Pull Request

### Convenciones de Commits

```
feat: Nueva característica
fix: Corrección de bug
docs: Cambios en documentación
style: Formato, punto y coma faltantes, etc
refactor: Refactorización de código
test: Añadir tests
chore: Actualizar tareas de build, configuración, etc
```

---

## 📊 Estadísticas del Proyecto

<div align="center">

![GitHub language count](https://img.shields.io/github/languages/count/Yonderus/API-PUBG?style=flat-square)
![GitHub top language](https://img.shields.io/github/languages/top/Yonderus/API-PUBG?style=flat-square)
![GitHub code size](https://img.shields.io/github/languages/code-size/Yonderus/API-PUBG?style=flat-square)
![GitHub last commit](https://img.shields.io/github/last-commit/Yonderus/API-PUBG?style=flat-square)

</div>

---

## 📝 Licencia

Este proyecto está bajo la Licencia MIT. Ver el archivo `LICENSE` para más detalles.

---

## 👨‍💻 Autor

<div align="center">

**Yonderus**

[![GitHub](https://img.shields.io/badge/GitHub-100000?style=for-the-badge&logo=github&logoColor=white)](https://github.com/Yonderus)

</div>

---

<div align="center">

### ⭐ Si te gustó este proyecto, considera darle una estrella!

<img src="https://capsule-render.vercel.app/api?type=waving&color=gradient&customColorList=12,14,18&height=100&section=footer" width="100%"/>

**Hecho con ❤️ y C#**

</div>
