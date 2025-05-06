INSERT INTO `PSI_NIAUMA`.`Particulier` (`Numero`, `Nom`, `prenom`, `Rue`, `Numerorue`, `Codepostal`, `Ville`, `Tel`, `Email`, `Metro`) 
VALUES 
('1', 'Durand', 'Medhy', 'Rue Cardinet', '15', '75017', 'Paris', '1234567890', 'Mdurand@gmail.com', 'Cardinet'),
('2','Durand', 'Marie','Rue de la république','30','75011','Paris','1234567890','Mdupond@gmail.com','République');






INSERT INTO plat (nom_plat_, type, quantité, date_de_péremption, PPP, date_de_création, origine,régime_plat)
VALUES 
('Raclette', 'Plat', 6, '2025-01-15', 10, '2025-01-10', 'Française','carnivore'),
('Salade de fruit', 'Dessert', 6, '2025-01-15', 5, '2025-01-10', 'Indifférent','végétarien'),
('Spaghetti Carbonara', 'Plat', 8, '2025-02-20', 12, '2025-02-15', 'Italienne', 'carnivore'),
('Tarte aux pommes', 'Dessert', 5, '2025-03-05', 7, '2025-02-28', 'Française', 'végétarien'),
('Sushi saumon', 'Plat', 10, '2025-01-25', 15, '2025-01-20', 'Japonaise', 'carnivore'),
('Soupe miso', 'Entrée', 6, '2025-04-10', 6, '2025-04-05', 'Japonaise', 'vegan'),
('Burger végétarien', 'Plat', 7, '2025-03-15', 9, '2025-03-10', 'Américaine', 'végétarien'),
('Couscous royal', 'Plat', 9, '2025-02-28', 14, '2025-02-25', 'Marocaine', 'carnivore'),
('Ratatouille', 'Plat', 8, '2025-05-01', 10, '2025-04-25', 'Française', 'vegan'),
('Pavlova', 'Dessert', 4, '2025-04-20', 11, '2025-04-15', 'Australienne', 'végétarien');



INSERT INTO Ingrédients_ (nom, régime)
VALUES 
('raclette fromage', 'Française'),
('pommes_de_terre', 'Française'),
('jambon', 'Française'),
('cornichon', 'Française'),
('fraise', 'Végétarien'),
('kiwi', 'Végétarien'),
('sucre', 'Végétarien'),
('pâtes', 'Italienne'),
('œufs', 'Carnivore'),
('lardons', 'Carnivore'),
('parmesan', 'Italienne'),
('pommes', 'Végétarien'),
('tarte', 'Végétarien'),    
('riz', 'Japonaise'),
('saumon', 'Carnivore'),
('algues', 'Japonaise'),
('tofu', 'Vegan'),
('pois chiches', 'Vegan'),
('bœuf', 'Carnivore'),
('carottes', 'Végétarien'),
('tomates', 'Végétarien'),
('aubergines', 'Vegan'),
('crème chantilly', 'Végétarien');


INSERT INTO Client (id_Client,mot_de_passe,pseudoCl,metrocl)
Values
('1','motpasse','jeremy','chatelet');


INSERT INTO Cuisinier (id_Cuisinier,mot_de_passe,pseudoCu,metrocui)
Values
('1','mddp','philippe','pont neuf'),
('2', 'password123', 'gordon', 'ramsay street'),
('3', 'securepass', 'jamie', 'oliver avenue'),
('4', 'chefpass', 'bocuse', 'lyon center'),
('5', 'cookingking', 'ratatouille', 'paris gourmet');



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
('Salade de fruit', 'sucre'),
('Spaghetti Carbonara', 'pâtes'),
('Spaghetti Carbonara', 'œufs'),
('Spaghetti Carbonara', 'lardons'),
('Spaghetti Carbonara', 'parmesan'),
('Tarte aux pommes', 'pommes'),
('Tarte aux pommes', 'tarte'),
('Sushi saumon', 'riz'),
('Sushi saumon', 'saumon'),
('Sushi saumon', 'algues'),
('Soupe miso', 'tofu'),
('Soupe miso', 'algues'),
('Burger végétarien', 'tofu'),
('Burger végétarien', 'tomates'),
('Burger végétarien', 'carottes'),
('Couscous royal', 'bœuf'),
('Couscous royal', 'pois chiches'),
('Couscous royal', 'carottes'),
('Ratatouille', 'aubergines'),
('Ratatouille', 'tomates'),
('Ratatouille', 'carottes'),
('Pavlova', 'crème chantilly'),
('Pavlova', 'sucre');


INSERT INTO contient (nom_plat_, id_commande)
VALUES 
('Raclette', '1'),
('Salade de fruit', '2'),
('Spaghetti Carbonara', 1),
('Tarte aux pommes', 1),
('Sushi saumon', 2),
('Soupe miso', 2),
('Burger végétarien', 1),
('Couscous royal', 2),
('Ratatouille', 2),
('Pavlova', 1);

INSERT INTO livre (id_commande,date_livraison, id_cuisinier,id_client)
VALUES 
('1','2025-01-12', '1','1'),
('2','2025-01-12', '1','1');







