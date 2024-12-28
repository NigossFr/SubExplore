# SubExplore

Application mobile communautaire dédiée aux sports sous-marins (plongée, apnée, randonnée aquatique).

## 🌊 Présentation

SubExplore est une application mobile qui permet aux pratiquants de sports sous-marins de :
- Découvrir et partager des spots de pratique
- Trouver des commerces et professionnels du secteur
- Partager des expériences via un format magazine

## 🛠 Technologies

- Frontend : .NET MAUI
- Backend : ASP.NET Core Web API
- Base de données : Azure SQL Database
- Services Cloud : Azure (Maps, SignalR, B2C, Search Service, CDN)

## 🚀 Installation

### Prérequis

- Visual Studio 2022 avec charge de travail .NET MAUI
- SDK .NET 7.0 ou supérieur
- Compte Azure (pour les services cloud)

### Configuration du projet

1. Cloner le repository
```bash
git clone https://github.com/votre-username/SubExplore.git
```

2. Restaurer les packages NuGet
```bash
dotnet restore
```

3. Configurer les secrets
- Copier `appsettings.template.json` vers `appsettings.Development.json`
- Remplir les valeurs de configuration nécessaires

4. Lancer l'application
```bash
dotnet run
```

## 🧪 Tests

```bash
dotnet test
```

## 🤝 Contribution

1. Forker le projet
2. Créer une branche pour votre fonctionnalité (`git checkout -b feature/AmazingFeature`)
3. Commiter vos changements (`git commit -m 'Add some AmazingFeature'`)
4. Pousser vers la branche (`git push origin feature/AmazingFeature`)
5. Ouvrir une Pull Request

## 📄 License

Ce projet est sous licence MIT - voir le fichier [LICENSE.md](LICENSE.md) pour plus de détails.

## ✨ Équipe

- [Votre nom] - Développeur principal
