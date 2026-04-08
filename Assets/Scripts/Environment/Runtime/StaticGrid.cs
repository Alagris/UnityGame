using Unity.Mathematics;
using UnityEngine;

namespace Env.Runtime
{
    public class StaticGrid : ProcEnvTerrain
    {
        [SerializeField]
        internal int Rows = 3;
        [SerializeField]
        internal int Columns = 3;

        // Start is called once before the first execution of Update after the MonoBehaviour is created

        Section[] sections;



        protected override void Start()
        {
            base.Start();
            Refresh();

        }
        public override void Clear()
        {
            base.Clear();
            sections = null;


        }
        public override void UnloadAll()
        {
            if (sections != null)
            {
                for (int i = 0; i < sections.Length; i++)
                {
                    sections[i].OnUnload();
                }
            }
        }
        public override void Refresh()
        {

            UnloadAll();
            if (sections == null)
            {
                sections = new Section[Rows * Columns];
            }
            else
            {
                if (sections.Length != Rows * Columns)
                {
                    Section[] old = sections;
                    sections = new Section[Rows * Columns];
                    int common = Mathf.Min(old.Length, sections.Length);
                    for (int i = 0; i < common; i++)
                    {
                        sections[i] = old[i];

                    }

                    for (int i = common; i < old.Length; i++)
                    {
                        old[i].DestroyImmediate();
                    }
                }
            }
            int2 offset = new int2(Columns / 2, Rows / 2);
            for (int x = 0, i = 0; x < Columns; x++)
            {
                for (int y = 0; y < Rows; y++, i++)
                {
                    if (sections[i] == null)
                    {
                        sections[i] = Spawner.SpawnSection(i);
                    }
                    sections[i].OnLoad(0, new int2(x, y) - offset);
                }
            }
        }


    }
}