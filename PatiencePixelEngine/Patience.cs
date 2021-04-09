using NewEngines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace PatiencePixelEngine
{
	class Patience : PixelEngine
	{
		List<Card>[] tableau;
		Queue<Card> reserve;
		Stack<Card>[] foundations;
		Stack<Card> waste;
		int cardsBy;
		int xSelection;
		int ySelection;
		int xSelection2;
		bool selected;
		bool changed = false;
		//readonly Sprite table = new(Sprite.LoadTextureFromFile(@"C:\Users\TeDeMos\Desktop\Cards\table.png", null));
		//readonly Sprite cardsFront = new(Sprite.LoadTextureFromFile(@"C:\Users\TeDeMos\Desktop\Cards\cardsFront.png", new(255, 0, 255)));
		//readonly Sprite cardsBack = new(Sprite.LoadTextureFromFile(@"C:\Users\TeDeMos\Desktop\Cards\cardsReverse.png", new(255, 0, 255)));
		readonly Sprite table = new(Sprite.LoadTextureFromFile(@"table.png", null));
		readonly Sprite cardsFront = new(Sprite.LoadTextureFromFile(@"cardsFront.png", new(255, 0, 255)));
		readonly Sprite cardsBack = new(Sprite.LoadTextureFromFile(@"cardsReverse.png", new(255, 0, 255)));
		Dictionary<Key, bool> prevPressed = new() { { Key.A, false }, { Key.S, false }, { Key.D, false }, { Key.W, false }, { Key.Q, false }, { Key.E, false }, { Key.T, false }, { Key.R, false }, { Key.Space, false }, { Key.Escape, false } };

		public Patience() : base(320, 303, 3, 3) => Window.Title = "Patience";

		public override bool OnCreate()
		{
			Reset();
			Draw();
			return true;
		}

		public override bool OnLoop(long timeEllapsed)
		{
			CheckKeys();
			if (changed)
			{
				Draw();
				changed = false;
			}
			return true;
		}

		void Draw()
		{
			DrawSprite(0, 0, table);
			for (int i = 0; i < 7; i++)
			{
				for (int j = 0; j < tableau[i].Count; j++)
				{
					DrawCard(35 * i + 2, 14 * j + 2, j == tableau[i].Count - 1, tableau[i][j]);
				}
			}
			if (reserve.Count > 0)
			{
				DrawCard(250, 254, true, reserve.Peek());
			}
			if (waste.Count > 0)
			{
				DrawCard(285, 254, true, waste.Peek());
			}
			for (int i = 0; i < 4; i++)
			{
				if (foundations[i].Count > 0)
				{
					DrawCard(i % 2 == 0 ? 250 : 285, i < 2 ? 153 : 202, true, foundations[i].Peek());
				}
			}
			DrawRect(35 * xSelection + 2, 14 * ySelection + 2, 32, 14 * (Math.Max(tableau[xSelection].Count - 1, 0) - ySelection) + 46, new(255, 0, 0));
			if (selected)
			{
				DrawRect(35 * xSelection2 + 2, 14 * Math.Max(tableau[xSelection2].Count - 1, 0) + 2, 32, 46, new(0, 255, 0));
			}
		}

		void Reset()
		{
			tableau = new int[7].Select(x => new List<Card>()).ToArray();
			reserve = Card.GetDeck(true);
			for (int i = 0; i < 7; i++)
			{
				for (int j = i; j < 7; j++)
				{
					tableau[j].Add(reserve.Dequeue());
					if (i == j)
					{
						tableau[j][^1] = tableau[j][^1].Flip();
					}
				}
			}
			foundations = new int[4].Select(x => new Stack<Card>()).ToArray();
			waste = new Stack<Card>();
			cardsBy = 3;
			xSelection = 0;
			ySelection = 0;
			xSelection2 = 0;
			selected = false;
		}

		bool CheckFoundationMove(Card c) => foundations[(int)c.Suit].Count == 0 && c.Rank == 1 || foundations[(int)c.Suit].Count > 0 && foundations[(int)c.Suit].Peek().Rank == c.Rank - 1;

		void FixYSelection()
		{
			if (ySelection >= tableau[xSelection].Count)
			{
				ySelection = Math.Max(tableau[xSelection].Count - 1, 0);
			}
			while (ySelection < tableau[xSelection].Count - 1 && tableau[xSelection][ySelection].Covered)
			{
				ySelection++;
			}
		}

		void MoveToFoundation(Card c) => foundations[(int)c.Suit].Push(c);

		void FlipLast()
		{
			if (tableau[xSelection].Count > 0 && tableau[xSelection][^1].Covered)
			{
				tableau[xSelection][^1] = tableau[xSelection][^1].Flip();
			}
		}
		
		void CheckKeys()
		{
			if (Keyboard.IsKeyDown(Key.W) && !prevPressed[Key.W] && !selected)
			{
				changed = true;
				prevPressed[Key.W] = true;
				if (ySelection > 0)
				{
					ySelection--;
				}
			}
			if (Keyboard.IsKeyDown(Key.S) && !prevPressed[Key.S] && !selected)
			{
				changed = true;
				prevPressed[Key.S] = true;
				if (ySelection < tableau[xSelection].Count - 1)
				{
					ySelection++;
				}
			}
			if (Keyboard.IsKeyDown(Key.A) && !prevPressed[Key.A])
			{
				changed = true;
				prevPressed[Key.A] = true;
				if (!selected)
				{
					if (xSelection > 0)
					{
						xSelection--;
						FixYSelection();
					}
				}
				else
				{
					if (xSelection2 > 0)
					{
						xSelection2--;
						if (xSelection2 == xSelection)
						{
							xSelection2--;
							if (xSelection2 < 0)
							{
								xSelection2 += 2;
							}
						}
					}
				}
			}
			if (Keyboard.IsKeyDown(Key.D) && !prevPressed[Key.D])
			{
				changed = true;
				prevPressed[Key.D] = true;
				if (!selected)
				{
					if (xSelection < 6)
					{
						xSelection++;
						FixYSelection();
					}
				}
				else
				{
					if (xSelection2 < 6)
					{
						xSelection2++;
						if (xSelection2 == xSelection)
						{
							xSelection2++;
							if (xSelection2 > 6)
							{
								xSelection2 -= 2;
							}
						}
					}
				}
			}
			if (Keyboard.IsKeyDown(Key.Q) && !prevPressed[Key.Q] && !selected)
			{
				changed = true;
				prevPressed[Key.Q] = true;
				if (waste.Count > 0)
				{
					if (tableau[xSelection].Count == 0 && waste.Peek().Rank == 13 || tableau[xSelection].Count > 0 && waste.Peek().CanPlaceOn(tableau[xSelection][^1]))
					{
						tableau[xSelection].Add(waste.Pop());
					}
				}

			}
			if (Keyboard.IsKeyDown(Key.E) && !prevPressed[Key.E] && !selected)
			{
				changed = true;
				prevPressed[Key.E] = true;
				if (tableau[xSelection].Count > 0)
				{
					if (CheckFoundationMove(tableau[xSelection][^1]))
					{
						MoveToFoundation(tableau[xSelection][^1]);
						tableau[xSelection].RemoveAt(tableau[xSelection].Count - 1);
						FlipLast();
						FixYSelection();
					}
				}
			}
			if (Keyboard.IsKeyDown(Key.T) && !prevPressed[Key.T] && !selected)
			{
				changed = true;
				prevPressed[Key.T] = true;
				if (waste.Count > 0)
				{
					if (CheckFoundationMove(waste.Peek()))
					{
						MoveToFoundation(waste.Pop());
					}
				}
			}
			if (Keyboard.IsKeyDown(Key.R) && !prevPressed[Key.R] && !selected)
			{
				changed = true;
				prevPressed[Key.R] = true;
				if (reserve.Count > 0)
				{
					for (int i = 0; i < cardsBy; i++)
					{
						if (reserve.Count > 0)
						{
							waste.Push(reserve.Dequeue().Flip());
						}
					}
				}
				else if (cardsBy > 1)
				{
					cardsBy--;
					reserve = new(waste.Reverse().Select(x => x.Flip()));
					waste = new();
				}
			}
			if (Keyboard.IsKeyDown(Key.Space) && !prevPressed[Key.Space])
			{
				changed = true;
				prevPressed[Key.Space] = true;
				if (!selected)
				{
					if (tableau[xSelection].Count > 0 && !tableau[xSelection][ySelection].Covered)
					{
						selected = true;
						xSelection2 = xSelection == 0 ? 1 : 0;
						for (int i = 0; i < 7; i++)
						{
							if (i == xSelection)
							{
								continue;
							}
							if (tableau[i].Count == 0 && tableau[xSelection][ySelection].Rank == 13 || tableau[i].Count > 0 && tableau[xSelection][ySelection].CanPlaceOn(tableau[i][^1]))
							{
								xSelection2 = i;
								break;
							}
						}
					}
				}
				else
				{
					if (tableau[xSelection2].Count == 0 && tableau[xSelection][ySelection].Rank == 13 || tableau[xSelection2].Count > 0 && tableau[xSelection][ySelection].CanPlaceOn(tableau[xSelection2][^1]))
					{
						int newY = tableau[xSelection2].Count;
						while(tableau[xSelection].Count > ySelection)
						{
							tableau[xSelection2].Add(tableau[xSelection][ySelection]);
							tableau[xSelection].RemoveAt(ySelection);
						}
						FlipLast();
						selected = false;
						xSelection = xSelection2;
						ySelection = newY;
					}
				}
			}
			if (Keyboard.IsKeyDown(Key.Escape) && !prevPressed[Key.Escape] && selected)
			{
				changed = true;
				prevPressed[Key.Escape] = true;
				selected = false;
			}
			foreach (KeyValuePair<Key, bool> pair in prevPressed)
			{
				if (!Keyboard.IsKeyDown(pair.Key))
				{
					prevPressed[pair.Key] = false;
				}
			}
		}

		void DrawCard(int x, int y, bool full, Card c)
		{
			if (c.Covered)
			{
				DrawPartSprite(x, y, 0, 0, 33, full ? 47 : 15, cardsBack);
			}
			else
			{
				DrawPartSprite(x, y, 33 * (c.Rank - 1), 47 * (int)c.Suit, 33, full ? 47 : 15, cardsFront);
			}
		}
	}

	struct Card
	{
		public enum CardSuit { Hearts, Diamonds, Clubs, Spades }
		public int Rank { get; init; }
		public CardSuit Suit { get; init; }
		public bool Covered { get; init; }

		public Card(int rank, CardSuit suit, bool covered)
		{
			Rank = rank;
			Suit = suit;
			Covered = covered;
		}

		public Card Flip() => new(Rank, Suit, !Covered);

		public bool CanPlaceOn(Card c) => c.Rank == Rank + 1 && ((int)c.Suit >= 2 && (int)Suit < 2 || (int)c.Suit < 2 && (int)Suit >= 2);

		public static Queue<Card> GetDeck(bool shuffled)
		{
			List<Card> deck = new();
			for (int i = 1; i < 14; i++)
			{
				for (int j = 0; j < 4; j++)
				{
					deck.Add(new Card(i, (CardSuit)j, true));
				}
			}
			if (shuffled)
			{
				Random r = new();
				deck = deck.OrderBy(x => r.Next()).ToList();
			}
			return new(deck);
		}
	}
}
