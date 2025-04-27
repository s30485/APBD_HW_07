-- Created by Vertabelo (http://vertabelo.com)
-- Last modification date: 2025-04-26 20:58:03.965

-- tables
-- Table: Device
CREATE TABLE Device (
                        Id varchar  NOT NULL,
                        Name varchar  NOT NULL,
                        IsEnabled smallint  NOT NULL,
                        CONSTRAINT Device_pk PRIMARY KEY  (Id)
);

-- Table: Embedded
CREATE TABLE Embedded (
                          Id int  NOT NULL,
                          IpAddress nvarchar  NOT NULL,
                          NetworkName nvarchar  NOT NULL,
                          DeviceId varchar  NOT NULL,
                          CONSTRAINT Embedded_pk PRIMARY KEY  (Id)
);

-- Table: PersonalComputer
CREATE TABLE PersonalComputer (
                                  Id int  NOT NULL,
                                  OperationSystem nvarchar  NOT NULL,
                                  DeviceId varchar  NOT NULL,
                                  CONSTRAINT PersonalComputer_pk PRIMARY KEY  (Id)
);

-- Table: Smartwatch
CREATE TABLE Smartwatch (
                            Id int  NOT NULL,
                            BatteryPercentage int  NOT NULL,
                            DeviceId varchar  NOT NULL,
                            CONSTRAINT Smartwatch_pk PRIMARY KEY  (Id)
);

-- foreign keys
-- Reference: Embedded_Device (table: Embedded)
ALTER TABLE Embedded ADD CONSTRAINT Embedded_Device
    FOREIGN KEY (DeviceId)
        REFERENCES Device (Id);

-- Reference: PersonalComputer_Device (table: PersonalComputer)
ALTER TABLE PersonalComputer ADD CONSTRAINT PersonalComputer_Device
    FOREIGN KEY (DeviceId)
        REFERENCES Device (Id);

-- Reference: Smartwatch_Device (table: Smartwatch)
ALTER TABLE Smartwatch ADD CONSTRAINT Smartwatch_Device
    FOREIGN KEY (DeviceId)
        REFERENCES Device (Id);

-- End of file.

