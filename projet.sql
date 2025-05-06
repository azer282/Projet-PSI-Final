INSERT INTO `projet`.`Particulier` (`Numero`, `Nom`, `prenom`, `Rue`, `Numerorue`, `Codepostal`, `Ville`, `Tel`, `Email`, `Metro`) 
VALUES 
('1', 'Durand', 'Medhy', 'Rue Cardinet', '15', '75017', 'Paris', '1234567890', 'Mdurand@gmail.com', 'Cardinet'),
('2','Durand', 'Marie','Rue de la république','30','75011','Paris','1234567890','Mdupond@gmail.com','République');






INSERT INTO plat (nom_plat_, type, quantité, date_de_péremption, PPP, date_de_création, origine,régime_plat)
VALUES 
('Raclette', 'Plat', 6, '2025-01-15', 10, '2025-01-10', 'Française','carnivore'),
('Salade de fruit', 'Dessert', 6, '2025-01-15', 5, '2025-01-10', 'Indifférent','végétarien');

INSERT INTO Ingrédients_ (nom, régime)
VALUES 
('raclette fromage', 'Française'),
('pommes_de_terre', 'Française'),
('jambon', 'Française'),
('cornichon', 'Française'),
('fraise', 'Végétarien'),
('kiwi', 'Végétarien'),
('sucre', 'Végétarien');

INSERT INTO Client (id_Client,mot_de_passe)
Values
('1','motpasse');


INSERT INTO Cuisinier (id_Cuisinier,mot_de_passe)
Values
('1','mddp');



INSERT INTO Commande (id_Commande) 
VALUES 
('1'),
('2');


INSERT INTO est_composé_de (nom_plat_, nom)
VALUES 
('Raclette', 'raclette fromage'),
('Raclette', 'pommes_de_terre'),
('Raclette', 'jambon'),
('Raclette', 'cornichon'),
('Salade de fruit', 'fraise'),
('Salade de fruit', 'kiwi'),
('Salade de fruit', 'sucre');

INSERT INTO contient (nom_plat_, id_commande)
VALUES 
('Raclette', '1'),
('Salade de fruit', '2');

INSERT INTO livre (id_commande,date_livraison, id_cuisinier)
VALUES 
('1','2025-01-12', '1'),
('2','2025-01-12', '1');







