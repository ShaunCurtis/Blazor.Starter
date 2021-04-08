It all depends on the relationship between `CrewOnLeave` and `CrewObBoard`.  If they aren't two incarnations of the same base class then you need to define an interface that the both share.  In the example the interface is blank which still works.

There's a simplified version of your code below, with to demo various methods of dealing with interface to class and class to interface convertions.

```csharp

@page "/Crewzing"
<table>
    <tr>
        @if (_hasCrews)
        {
            <td @onclick="SearchCrews">
                <span style="cursor: pointer;" data-toggle="modal"
                      data-target="#@(modalId)">
                    @Crews.Count()
                </span>
            </td>
        }
        else
        {
            <td></td>
        }

    </tr>
</table>

@code {

    public List<ICrew> Crews => CrewsOnLeave.Count > 0 ? CrewsOnLeave.Select(item => (ICrew)item).ToList() : CrewsOnBoard.Select(item => (ICrew)item).ToList();

    [Parameter] public EventCallback<List<ICrew>> CrewsClicked { get; set; }

    private bool _hasCrews => (this.Crews != null && this.Crews.Count > 0);

    private string modalId = "xxx";

    private void SearchCrews()
    {
        GetCrewType();
        CrewsClicked.InvokeAsync(Crews);
    }

    public interface ICrew
    {
    }

    public class CrewOnBoard : ICrew
    {
        public string Boat { get; set; }
    }

    public class CrewOnLeave : ICrew
    {
        public string Boat { get; set; }
    }

    public List<CrewOnBoard> CrewsOnBoard = new List<CrewOnBoard>()
{
        new CrewOnBoard() { Boat = "Cyclops"},
        new CrewOnBoard() { Boat = "Sirius"}
    };

    public List<CrewOnLeave> CrewsOnLeave = new List<CrewOnLeave>()
{
        new CrewOnLeave() { Boat = "Medusa"},
        new CrewOnLeave() { Boat = "Alderbaran"}
    };

    private void GetCrewType()
    {
        var c = Crews[0] ?? null;
        if (c is CrewOnBoard)
        {
            var crew = Crews.Select(item => (CrewOnBoard)item).ToList();
        }
        else if (c is CrewOnLeave)
        {
            var crew = Crews.Select(item => (CrewOnLeave)item).ToList();
        }
    }
}
```

