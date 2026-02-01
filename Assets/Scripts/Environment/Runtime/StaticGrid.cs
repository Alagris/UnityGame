using Unity.Mathematics;
using UnityEngine;

public class StaticGrid : ProcSectionSpawner
{
    [SerializeField]
    internal int Rows = 3;
    [SerializeField]
    internal int Columns = 3;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    Section[] sections;
    


    private void Start()
    {
        Refresh();

    }
    public override void Refresh()
    {

        if (sections != null)
        {
            for (int i = 0; i < sections.Length; i++)
            {
                sections[i].OnExit();
            }
        }
        sections = new Section[Rows * Columns];
        int2 offset = new int2(Columns / 2, Rows / 2);
        for (int x = 0, i = 0; x < Columns; x++)
        {
            for (int y = 0; y < Rows; y++, i++)
            {
                sections[i] = SpawnSection(i);
                sections[i].OnLoad(0, new int2(x, y) - offset);
            }
        }
    }

    
}
