INSERT OR IGNORE INTO Project (id, thumbnail, name, description) VALUES
(1, "avares://ProjectPlan/Assets/avalonia-logo.ico", "Avalonia Test", "Avalonia est un framework UI multiplateforme pour .NET, inspiré de WPF. Il te permet de créer des applications modernes et performantes pour Windows, Linux et macOS."),
(2, "avares://ProjectPlan/Assets/Thales.png", "Thales Test", "Thales est une entreprise française spécialisée dans l'aérospatiale, la défense, la sécurité et le transport terrestre. Elle conçoit et fabrique des systèmes et des équipements pour les marchés civils et militaires."),
(3, "avares://ProjectPlan/Assets/Docker.png", "Docker Test", "Docker est une plateforme de conteneurisation qui permet aux développeurs de créer, déployer et exécuter des applications dans des conteneurs légers et portables. Docker facilite la gestion des dépendances et la distribution des applications.");

INSERT OR IGNORE INTO Column (id, id_project, name) VALUES
(1, 1, "To Do"),
(2, 1, "In Progress"),
(3, 1, "Done"),
(4, 2, "Backlog"),
(5, 2, "In Development"),
(6, 2, "Completed"),
(7, 3, "Ideas"),
(8, 3, "Development"),
(9, 3, "Release");

INSERT OR IGNORE INTO Card (id, id_column, title) VALUES
(1, 1, "Learn Avalonia"),
(2, 1, "Build a sample app"),
(3, 2, "Design the UI"),
(4, 2, "Implement the ViewModel"),
(5, 3, "Test the application"),
(6, 3, "Deploy to production"),
(7, 4, "Gather requirements"),
(8, 4, "Create user stories"),
(9, 5, "Develop features"),
(10, 5, "Fix bugs"),
(11, 6, "Perform code review"),
(12, 6, "Merge to main branch"),
(13, 7, "Brainstorm new ideas"),
(14, 7, "Evaluate feasibility"),
(15, 8, "Start development"),
(16, 8, "Track progress"),
(17, 9, "Prepare release notes"),
(18, 9, "Publish new version");