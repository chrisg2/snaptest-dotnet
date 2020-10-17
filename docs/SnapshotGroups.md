# Snapshot groups

> _TODO: Documentation to be written_

Snapshot groups can be used to store a group of snapshotted values in a single snapshot file. Each snapshotted value is identified by a unique key.


## To be noted

If using snapshot groups, ensure a consistent `IndentJson` value is used for all snapshots within a group - otherwise indentation in the snapshot file may change based on which actual snapshot was written to the snapshot file most recently. Recommend to always use `IndentJson=true` with snapshot groups.

See notes on mismatched actual files and snapshot groups in CreatingAndUpdatingSnapshots.md.
