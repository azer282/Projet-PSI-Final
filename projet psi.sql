drop database if exists projet;
create database projet;
use projet;

CREATE TABLE Particulier(
   
   Numero VARCHAR(50),
   Nom VARCHAR(50),
   prenom VARCHAR(50),
   Rue VARCHAR(50),
   NumeroRue VARCHAR(50),
   Codepostal VARCHAR(50),
   Ville VARCHAR(50),
   Tel VARCHAR(50),
   Email VARCHAR(50),
   Metro VARCHAR(50),
   PRIMARY KEY(Email)
);

CREATE TABLE Entreprise(
   identifiant VARCHAR(50),
   nom_entreprise VARCHAR(50),
   référent VARCHAR(50),
   mot_de_passe VARCHAR(50),
   PRIMARY KEY(identifiant)
);

CREATE TABLE plat(
   nom_plat_ VARCHAR(50),
   type VARCHAR(50),
   quantité INTEGER(50),
   date_de_péremption  DATE,
   PPP INTEGER(50),
   date_de_création DATE,
   origine VARCHAR(50),
   régime_plat VARCHAR(50),
   PRIMARY KEY(nom_plat_)
);

CREATE TABLE Ingrédients_(
   nom VARCHAR(50),
   régime VARCHAR(50),
   PRIMARY KEY(nom)
);

CREATE TABLE Commande(
   id_commande VARCHAR(50),
   PRIMARY KEY(id_commande)
);

CREATE TABLE cuisinier(
mot_de_passe VARCHAR(50),
   id_cuisinier VARCHAR(50),
   PRIMARY KEY(id_cuisinier)
);

CREATE TABLE client(
mot_de_passe VARCHAR(50),
   id_client VARCHAR(50),
   PRIMARY KEY(id_client)
);

CREATE TABLE est_composé_de(
   nom_plat_ VARCHAR(50),
   nom VARCHAR(50),
   PRIMARY KEY(nom_plat_, nom),
   FOREIGN KEY(nom_plat_) REFERENCES plat(nom_plat_),
   FOREIGN KEY(nom) REFERENCES Ingrédients_(nom)
);

CREATE TABLE publie(
   nom_plat_ VARCHAR(50),
   id_cuisinier VARCHAR(50),
   PRIMARY KEY(nom_plat_, id_cuisinier),
   FOREIGN KEY(nom_plat_) REFERENCES plat(nom_plat_),
   FOREIGN KEY(id_cuisinier) REFERENCES cuisinier(id_cuisinier)
);

CREATE TABLE livre(
   id_commande VARCHAR(50),
   id_cuisinier VARCHAR(50),
   date_livraison VARCHAR(50),
   PRIMARY KEY(id_commande, id_cuisinier),
   FOREIGN KEY(id_commande) REFERENCES Commande(id_commande),
   FOREIGN KEY(id_cuisinier) REFERENCES cuisinier(id_cuisinier)
);


CREATE TABLE orderr(
   identifiant VARCHAR(50),
   id_commande VARCHAR(50),
   PRIMARY KEY(identifiant, id_commande),
   FOREIGN KEY(identifiant) REFERENCES Entreprise(identifiant),
   FOREIGN KEY(id_commande) REFERENCES Commande(id_commande)
);

CREATE TABLE contient(
   nom_plat_ VARCHAR(50),
   id_commande VARCHAR(50),
   PRIMARY KEY(nom_plat_, id_commande),
   FOREIGN KEY(nom_plat_) REFERENCES plat(nom_plat_),
   FOREIGN KEY(id_commande) REFERENCES Commande(id_commande)
);

CREATE TABLE avis_entreprise(
   identifiant VARCHAR(50),
   id_cuisinier VARCHAR(50),
   note VARCHAR(50),
   commentaire VARCHAR(50),
   date_avis DATE,
   PRIMARY KEY(identifiant, id_cuisinier),
   FOREIGN KEY(identifiant) REFERENCES Entreprise(identifiant),
   FOREIGN KEY(id_cuisinier) REFERENCES cuisinier(id_cuisinier)
);

CREATE TABLE est_cuisinier(
   identifiant VARCHAR(50),
   id_cuisinier VARCHAR(50),
   PRIMARY KEY(identifiant, id_cuisinier),
   FOREIGN KEY(identifiant) REFERENCES Particulier(Email),
   FOREIGN KEY(id_cuisinier) REFERENCES cuisinier(id_cuisinier)
);

CREATE TABLE est_client(
   identifiant VARCHAR(50),
   id_client VARCHAR(50),
   PRIMARY KEY(identifiant, id_client),
   FOREIGN KEY(identifiant) REFERENCES Particulier(Email),
   FOREIGN KEY(id_client) REFERENCES client(id_client)
);

CREATE TABLE avis_client(
   id_cuisinier VARCHAR(50),
   id_client VARCHAR(50),
   PRIMARY KEY(id_cuisinier, id_client),
   FOREIGN KEY(id_cuisinier) REFERENCES cuisinier(id_cuisinier),
   FOREIGN KEY(id_client) REFERENCES client(id_client)
);
