# Snapshot file format

Snapshot files are text files. The specific details of the value stored in a snapshot file depend on the type of the actual value provided for a snapshot match operation:

- Values that are strings and primitive types (that is, values of type int, bool, etc) are stored verbatim with an extra `Environment.NewLine` at the end.

- Guids are stored as text in the pattern `XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX` where X is a hex digit (0,1,2,3,4,5,6,7,8,9,A,B,C,D,E,F), with an `Enviornment.NewLine` at the end

- Other objects are serialized in JSON format. Properties are generally ordered by name when SnapTest saves a value to a snapshot file, although in keeping with regular JSON semantics the order that properties appear is not important for the purposes of matching actual values to snapshotted values.


## Line endings

Line endings in snapshot files should match the convention for text file line endings on your operating system: CR+LF on Windows, and CR on UNIX-like operating systems.

A snapshot file having incorrect line endings for the platform may cause matchines of simple values with the snapshot to fail.

When working with snapshots files across different platforms, common source control systems can take care of ensuring line endings in checked out files are set appropriately. If  snapshot files are manually copied to and from Windows then it may be necessary to take explicit action to convert line endings.
