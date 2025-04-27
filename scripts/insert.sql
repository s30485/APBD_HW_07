insert into Device (Id, Name, IsEnabled) values
                                             ('SW-1', 'Apple Watch SE2', 1),
                                             ('P-1', 'LinuxPC', 0),
                                             ('P-2', 'ThinkPad T440', 1),
                                             ('ED-1', 'Pi2', 0),
                                             ('ED-2', 'Pi4', 1),
                                             ('SW-2', 'Sigma Epic Watch v2', 0);

-- Smartwatches
INSERT INTO Smartwatch (Id, BatteryPercentage, DeviceId) VALUES
                                                             (1,  27,   'SW-1'),   -- from original CSV: 27%
                                                             (2, 100,   'SW-2');   -- brand-new, give it 100% battery

-- Personal Computers
INSERT INTO PersonalComputer (Id, OperationSystem, DeviceId) VALUES
                                                                 (1, 'Linux Mint', 'P-1'),  -- original CSV said Linux Mint
                                                                 (2, '',          'P-2');   -- no OS specified, use empty string

-- Embedded Devices
INSERT INTO Embedded (Id, IpAddress, NetworkName, DeviceId) VALUES
                                                                (1, '192.168.1.44', 'MD Ltd.Wifi-1', 'ED-1'),  -- from original CSV
                                                                (2, '192.168.1.45', 'eduroam',       'ED-2');  -- from original CSV
