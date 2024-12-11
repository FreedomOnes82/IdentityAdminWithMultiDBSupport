Create TABLE IF NOT EXISTS `AspNetRoles` (
	`Id` varchar(450)  PRIMARY KEY,
    `Name` varchar(256) NULL,
	`NormalizedName` nvarchar(256) NULL,
	`ConcurrencyStamp` varchar(200) NULL
);

CREATE UNIQUE INDEX uk_aspnetrole_name ON AspNetRoles(name);
CREATE UNIQUE INDEX `RoleNameIndex` ON `AspNetRoles` (`NormalizedName`);

CREATE TABLE IF NOT EXISTS `AspNetRoleClaims` (
    `Id` INT AUTO_INCREMENT PRIMARY KEY,
    `RoleId` varchar(450) NOT NULL,
    `ClaimType` varchar(2048),
    `ClaimValue` varchar(2048),
    FOREIGN KEY (`RoleId`) REFERENCES `AspNetRoles`(`Id`)
);
   

CREATE TABLE IF NOT EXISTS `AspNetUsers` (
          `Id` varchar(95) CHARACTER SET utf8mb4 NOT NULL,
          `UserName` varchar(256) CHARACTER SET utf8mb4 NULL,
          `NormalizedUserName` varchar(256) CHARACTER SET utf8mb4 NULL,
          `Email` varchar(256) CHARACTER SET utf8mb4 NULL,
          `NormalizedEmail` varchar(256) CHARACTER SET utf8mb4 NULL,
          `EmailConfirmed` tinyint(1) NOT NULL,
          `PasswordHash` varchar(2048) CHARACTER SET utf8mb4 NULL,
          `SecurityStamp` varchar(100) CHARACTER SET utf8mb4 NULL,
          `ConcurrencyStamp` varchar(100) CHARACTER SET utf8mb4 NULL,
          `PhoneNumber` varchar(100) CHARACTER SET utf8mb4 NULL,
          `PhoneNumberConfirmed` tinyint(1) NOT NULL,
          `TwoFactorEnabled` tinyint(1) NOT NULL,
          `LockoutEnd` datetime NULL,
          `LockoutEnabled` tinyint(1) NOT NULL,
          `AccessFailedCount` int NOT NULL,
          CONSTRAINT `PK_AspNetUsers` PRIMARY KEY (`Id`)
      ) CHARACTER SET=utf8mb4;

      CREATE TABLE IF NOT EXISTS `AspNetRoleClaims` (
          `Id` int NOT NULL AUTO_INCREMENT,
          `RoleId` varchar(95) CHARACTER SET utf8mb4 NOT NULL,
          `ClaimType` longtext CHARACTER SET utf8mb4 NULL,
          `ClaimValue` longtext CHARACTER SET utf8mb4 NULL,
          CONSTRAINT `PK_AspNetRoleClaims` PRIMARY KEY (`Id`),
          CONSTRAINT `FK_AspNetRoleClaims_AspNetRoles_RoleId` FOREIGN KEY (`RoleId`) REFERENCES `AspNetRoles` (`Id`) ON DELETE CASCADE
      ) CHARACTER SET=utf8mb4;

       CREATE TABLE IF NOT EXISTS `AspNetUserClaims` (
          `Id` int NOT NULL AUTO_INCREMENT,
          `UserId` varchar(95) CHARACTER SET utf8mb4 NOT NULL,
          `ClaimType` longtext CHARACTER SET utf8mb4 NULL,
          `ClaimValue` longtext CHARACTER SET utf8mb4 NULL,
          CONSTRAINT `PK_AspNetUserClaims` PRIMARY KEY (`Id`),
          CONSTRAINT `FK_AspNetUserClaims_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `AspNetUsers` (`Id`) ON DELETE CASCADE
      ) CHARACTER SET=utf8mb4;

       CREATE TABLE IF NOT EXISTS `AspNetUserLogins` (
          `LoginProvider` varchar(128) CHARACTER SET utf8mb4 NOT NULL,
          `ProviderKey` varchar(128) CHARACTER SET utf8mb4 NOT NULL,
          `ProviderDisplayName` longtext CHARACTER SET utf8mb4 NULL,
          `UserId` varchar(95) CHARACTER SET utf8mb4 NOT NULL,
          CONSTRAINT `PK_AspNetUserLogins` PRIMARY KEY (`LoginProvider`, `ProviderKey`),
          CONSTRAINT `FK_AspNetUserLogins_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `AspNetUsers` (`Id`) ON DELETE CASCADE
      ) CHARACTER SET=utf8mb4;


       CREATE TABLE IF NOT EXISTS `AspNetUserRoles` (
          `UserId` varchar(95) CHARACTER SET utf8mb4 NOT NULL,
          `RoleId` varchar(95) CHARACTER SET utf8mb4 NOT NULL,
          CONSTRAINT `PK_AspNetUserRoles` PRIMARY KEY (`UserId`, `RoleId`),
          CONSTRAINT `FK_AspNetUserRoles_AspNetRoles_RoleId` FOREIGN KEY (`RoleId`) REFERENCES `AspNetRoles` (`Id`) ON DELETE CASCADE,
          CONSTRAINT `FK_AspNetUserRoles_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `AspNetUsers` (`Id`) ON DELETE CASCADE
      ) CHARACTER SET=utf8mb4;


        CREATE TABLE IF NOT EXISTS `AspNetUserTokens` (
          `UserId` varchar(95) CHARACTER SET utf8mb4 NOT NULL,
          `LoginProvider` varchar(128) CHARACTER SET utf8mb4 NOT NULL,
          `Name` varchar(128) CHARACTER SET utf8mb4 NOT NULL,
          `Value` longtext CHARACTER SET utf8mb4 NULL,
          CONSTRAINT `PK_AspNetUserTokens` PRIMARY KEY (`UserId`, `LoginProvider`, `Name`),
          CONSTRAINT `FK_AspNetUserTokens_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `AspNetUsers` (`Id`) ON DELETE CASCADE
      ) CHARACTER SET=utf8mb4;

      
      CREATE INDEX `IX_AspNetRoleClaims_RoleId` ON `AspNetRoleClaims` (`RoleId`);


      CREATE INDEX `IX_AspNetUserClaims_UserId` ON `AspNetUserClaims` (`UserId`);

      CREATE INDEX `IX_AspNetUserLogins_UserId` ON `AspNetUserLogins` (`UserId`);

      CREATE INDEX `IX_AspNetUserRoles_RoleId` ON `AspNetUserRoles` (`RoleId`);

      CREATE INDEX `EmailIndex` ON `AspNetUsers` (`NormalizedEmail`);

      CREATE UNIQUE INDEX `UserNameIndex` ON `AspNetUsers` (`NormalizedUserName`);

 