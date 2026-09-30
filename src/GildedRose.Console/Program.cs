using System.Collections.Generic;

namespace GildedRose.Console;

public class Program
{
    public IList<Item> Items = new List<Item>();

    static void Main(string[] args)
    {
        System.Console.WriteLine("OMGHAI!");

        var app = new Program()
                      {
                          Items = new List<Item>
                                      {
                                          new Item {Name = "+5 Dexterity Vest", SellIn = 10, Quality = 20},
                                          new Item {Name = "Aged Brie", SellIn = 2, Quality = 0},
                                          new Item {Name = "Elixir of the Mongoose", SellIn = 5, Quality = 7},
                                          new Item {Name = "Sulfuras, Hand of Ragnaros", SellIn = 0, Quality = 80},
                                          new Item
                                              {
                                                  Name = "Backstage passes to a TAFKAL80ETC concert",
                                                  SellIn = 15,
                                                  Quality = 20
                                              },
                                          new Item {Name = "Conjured Mana Cake", SellIn = 3, Quality = 6}
                                      }

                      };

        app.UpdateQuality();

        System.Console.ReadKey();
    }

    public void UpdateQuality()
    {
        foreach (var item in Items)
        {
            if (IsSulfuras(item))
            {
                continue;
            }

            if (IsBackstagePass(item))
            {
                UpdateBackstagePass(item);
            }
            else if (IsAgedBrie(item))
            {
                IncreaseQuality(item);
            }
            else if (IsConjured(item))
            {
                DecreaseQuality(item, 2);
            }
            else
            {
                DecreaseQuality(item);
            }

            item.SellIn--;

            if (item.SellIn < 0)
            {
                if (IsBackstagePass(item))
                {
                    item.Quality = 0;
                }
                else if (IsAgedBrie(item))
                {
                    IncreaseQuality(item);
                }
                else if (IsConjured(item))
                {
                    DecreaseQuality(item, 2);
                }
                else
                {
                    DecreaseQuality(item);
                }
            }
        }
    }

    private static bool IsSulfuras(Item item)
    {
        return item.Name == "Sulfuras, Hand of Ragnaros";
    }

    private static bool IsAgedBrie(Item item)
    {
        return item.Name == "Aged Brie";
    }

    private static bool IsBackstagePass(Item item)
    {
        return item.Name == "Backstage passes to a TAFKAL80ETC concert";
    }

    private static bool IsConjured(Item item)
    {
        return item.Name.StartsWith("Conjured");
    }

    private static void IncreaseQuality(Item item, int amount = 1)
    {
        item.Quality = Math.Min(50, item.Quality + amount);
    }

    private static void DecreaseQuality(Item item, int amount = 1)
    {
        item.Quality = Math.Max(0, item.Quality - amount);
    }

    private static void UpdateBackstagePass(Item item)
    {
        if (item.SellIn <= 5)
        {
            IncreaseQuality(item, 3);
        }
        else if (item.SellIn <= 10)
        {
            IncreaseQuality(item, 2);
        }
        else
        {
            IncreaseQuality(item);
        }
    }
}

public class Item
{
    public string Name { get; set; } = "";

    public int SellIn { get; set; }

    public int Quality { get; set; }
}
