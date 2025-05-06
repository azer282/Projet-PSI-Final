DESC Particulier;
-- décrit la table particulier

select nom_plat_,type,PPP from plat;
-- montre les plats leurs type et le prix par personne 

select nom_plat_,régime_plat from plat where régime_plat = 'Végétarien';
-- montre les plats qui sont végétariens 

select nom_plat_,régime_plat from  plat where régime_plat = 'carnivore';
-- montre les plat qui sont carnivore 

select ville, nom ,codepostal from Particulier;
-- montre données des particuliers 

SELECT * 
FROM Particulier
ORDER BY Nom ASC, prenom ASC;
-- trie les particulers par odre alphabétique 

select count(*) from Cuisinier;
-- compte le nombre de cuisiniers

select count(*) from Client;
-- compte le nombre de clients

select count(*) from Particulier;
-- compte le nombre de partticuliers

select * from Ingrédients_ where nom = 'raclette fromage' and régime = 'Française';
-- liste les ingrédients qui s'appelle raclette fromage et de régime française

select * from Particulier where Codepostal in('75011','75016','75017');
-- liste les propriétaires habitant dans les codepostaux suivants 

select * from plat where PPP < 10;
-- liste les plats qui ont un prix par personne inférieur a 10 euros
