# Snapshot file format

- Strings saved verbatim with an extra `Environment.NewLine` at the end

- Guids are stored as text in the pattern `XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX` where X is a hex digit (0,1,2,3,4,5,6,7,8,9,A,B,C,D,E,F), with an `Enviornment.NewLine` at the end

- Other objects are serialized in JSON format, with properties ordered by name
