-- Created by Vertabelo (http://vertabelo.com)
-- Last modification date: 2025-04-25 08:51:32.573

-- tables
-- Table: Device
CREATE TABLE Device (
                        id varchar  NOT NULL,
                        name varchar  NOT NULL,
                        is_enabled smallint  NOT NULL,
                        CONSTRAINT Device_pk PRIMARY KEY  (id)
);

-- Table: Embedded
CREATE TABLE Embedded (
                          id int  NOT NULL,
                          ip_address nvarchar  NOT NULL,
                          network_name nvarchar  NOT NULL,
                          device_id varchar  NULL,
                          CONSTRAINT Embedded_pk PRIMARY KEY  (id)
);

-- Table: PersonalComputer
CREATE TABLE PersonalComputer (
                                  id int  NOT NULL,
                                  operation_system nvarchar  NOT NULL,
                                  device_id varchar  NULL,
                                  CONSTRAINT PersonalComputer_pk PRIMARY KEY  (id)
);

-- Table: Smartwatch
CREATE TABLE Smartwatch (
                            id int  NOT NULL,
                            battery_percentage int  NOT NULL,
                            device_id varchar  NULL,
                            CONSTRAINT Smartwatch_pk PRIMARY KEY  (id)
);

-- foreign keys
-- Reference: Embedded_Device (table: Embedded)
ALTER TABLE Embedded ADD CONSTRAINT Embedded_Device
    FOREIGN KEY (device_id)
        REFERENCES Device (id);

-- Reference: PersonalComputer_Device (table: PersonalComputer)
ALTER TABLE PersonalComputer ADD CONSTRAINT PersonalComputer_Device
    FOREIGN KEY (device_id)
        REFERENCES Device (id);

-- Reference: Smartwatch_Device (table: Smartwatch)
ALTER TABLE Smartwatch ADD CONSTRAINT Smartwatch_Device
    FOREIGN KEY (device_id)
        REFERENCES Device (id);

-- End of file.

