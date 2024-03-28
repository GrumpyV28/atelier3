Mission Évaluation :

L'objectif principal de cette mission est de compiler le rapport, compléter le fichier README, et concevoir la page spécifique à cet atelier dans le portfolio.

Contenu de l'évaluation :

Tâche 1 (facultative)
Tâche 2
Tâche 3 (facultative)
Tâche 4
Tâche 5
Tâche 6
Tâche 7
ÉVALUATION

Application MediatekDocuments
Cette application a été élaborée pour simplifier la gestion des ressources documentaires (livres, DVD, revues) au sein des médiathèques. Développée en utilisant le langage C# avec Visual Studio 2019, cette application de bureau est conçue pour être déployée sur plusieurs postes ayant accès à une base de données centralisée.

Pour communiquer avec la base de données MySQL, l'application utilise une API REST. Des indications précises pour récupérer cette API, ainsi qu'un guide d'utilisation détaillé, sont disponibles ci-dessous.

Présentation
Actuellement, l'application est partiellement opérationnelle. Les fonctionnalités existantes comprennent la recherche et l'affichage d'informations sur les ressources documentaires de la médiathèque (livres, DVD, revues), ainsi que la possibilité d'enregistrer de nouveaux numéros de revues.

img1

L'interface utilisateur est structurée autour d'une fenêtre unique, divisée en plusieurs onglets.

Les onglets disponibles
Onglet 1 : Livres
Cet onglet présente la liste des livres, triée par défaut par titre. Les informations telles que le titre, l'auteur, la collection, le genre, le public et le rayon sont fournies.

img2

Fonctionnalités de recherche
Recherche par titre : Permet de trouver un ou plusieurs livres en saisissant leur titre, avec une fonction d'autocomplétion et sans distinction de casse.
Recherche par numéro : Permet de trouver un livre spécifique en saisissant son numéro.
Filtres
Il est possible d'appliquer un filtre (un à la fois) sur le genre, le public ou le rayon. Tout changement de filtre ou de recherche annule le filtre actif.

Tri
Le tri de la liste peut être effectué en cliquant sur le titre d'une colonne.

Affichage détaillé
Sélectionner un livre permet d'afficher ses détails dans la partie inférieure de la fenêtre, y compris son image.

Onglet 2 : DVD
Cet onglet fonctionne de manière similaire à l'onglet Livres, mais affiche la liste des DVD.

Onglet 3 : Revues
Comme les onglets précédents, mais pour les revues.

Onglet 4 : Parutions des revues
Ce volet permet d'enregistrer la réception de nouveaux numéros de revues.

La base de données 'mediatek86' est en format MySQL. Elle comprend deux types de documents : les "génériques" (Document, Revue, Livres-DVD, Livre et DVD) et les documents "physiques" (exemplaires de livres ou de DVD, numéros de revues ou de journaux).

img4

Pour plus de détails sur la structure de la base de données, veuillez vous référer au texte d'origine.

API REST
L'accès à la base de données se fait via une API REST sécurisée par une authentification de base. Le code de l'API est disponible ici, avec des instructions d'utilisation dans le fichier README.

Installation de l'application
Pour installer l'application, suivez ces étapes :

Installez Visual Studio 2019 Enterprise et les extensions Specflow et Newtonsoft.Json.
Téléchargez le code, décompressez-le et renommez le dossier en "mediatekdocuments".
Téléchargez et installez l'API REST nécessaire ainsi que la base de données en suivant les instructions fournies dans le README correspondant.
Une fois ces étapes effectuées, l'application sera opérationnelle et prête à être utilisée par le personnel de la médiathèque pour saisir les informations sur les documents.