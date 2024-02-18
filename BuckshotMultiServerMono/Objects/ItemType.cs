using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace BuckshotMultiServerMono.Objects
{
    public enum ItemType
    {
        None,
        Handcuff,
        Glass,
        Beer,
        Saw,
        Cigarette,
    }
    public static class ItemTypeGetter
    {
        public static ItemType Random()
        {
            ItemType[] choices = {ItemType.Handcuff, ItemType.Glass, ItemType.Beer,
                ItemType.Saw, ItemType.Cigarette};
            return choices[System.Random.Shared.Next(choices.Length)];
        }
        public static ItemType NotCig()
        {
            ItemType[] choices = {ItemType.Handcuff, ItemType.Glass, ItemType.Beer,
                ItemType.Saw};
            return choices[System.Random.Shared.Next(choices.Length)];
        }
    }
}
