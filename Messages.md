This JSON file contains everything the program will show on screen, picked randomly.
The file stored on this repository is the default file that the program will download if the local one in ~/.local/share/hackerterminal is missing.
# File structure
On the top level there are
- normal
- progressbars
- tasks
- spinners
## normal
This section contains regular messages that will be shown on screen without any other funky business going on.
For example, if the file contains this:
```json lines
"normal": [
        "message1",
        "message2"
]
```
then the output of the program would look something like this:
```
message1
message1
message2
message1
message2
message2
message1
message2
```
## progressbars
These appear as a progress bar that gradually fills up.

Progressbars have to be defined as an object like this:
```json lines
{
  "label": "Example",
  "type": "number",
  "max": 256,
  "wait": 500
}
```
Where
- **label** - the text that gets shown on screen
- **type** - the type of the progressbar
  - *percent* - shows the total percentage completed
  - *number* - shows the raw progress number
- **max** - the number to which the bar counts up from 0 (inclusive)
- **wait** - the number of millisecons 

### Examples
Waiting progressbar
```
Example         ░░░░█░░░░░ | ...
```
Working progressbar
```
Example         ██████░░░░ | 60%
Example         ██████░░░░ | 153/256
```
## tasks
These are similar to [normal messages](#normal), but wait for a couple seconds and display a result.
They can be defined as simple strings or an object for extra parameters:
```json
{
  "label": "Example",
  "failchance": 5
}
```

- **label** - the message of the task.
- **failchance** - the chance of a failure of the task (see below)

For example, a task `Eating vegetables` will be displayed as
```
Eating vegetables...
```
for a short time and either of these afterwards:
```
Eating vegetables... Done
Eating vegetables... FAIL
```
By default, failing has a 10% probability, but this can be overridden in the definition as demonstrated above (where it would instead have only 5%)
## spinners
Similar to the [tasks](#tasks) but cannot fail and display a spinner next to them.
They are defined as strings.

For example, a spinner `Working` would be displayed as
```
Working -
    ↓
Working \
    ↓
Working |
    ↓
Working /
    ↓
(repeat)
```
A spinner that has finished its job is shown as this:
```
Working +
```