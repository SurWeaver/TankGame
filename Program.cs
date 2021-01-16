using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TankGame
{
	interface ITank
	{
		int Armor { get; set; }
		float Damage { get; set; }
		NaturalNumber Ammo { get; set; }
		NaturalNumber Health { get; set; }

		void Shoot(ITank opponent);
		void TakeDamage(float damage);
		void Repair(int healthAmount);
		void BuyAmmo(int ammoAmount);
		void WriteInfo();
	}

	interface IComputer
	{
		void ComputerTurn(ITank opponent);
	}
	 

	class Tank : ITank
	{
		readonly TankTypes TankType;
		
		public int Armor { get; set; }
		public float Damage { get; set; }
		public NaturalNumber Ammo { get; set; }
		public NaturalNumber Health { get; set; }


		public void Shoot(ITank opponent)
		{
			// Создание исходного сообщения
			var message = $"Произведён выстрел на {Damage} урона.\n";
			var currentDamage = Damage;

			if (Ammo == 0)
			{
				if (TankType == TankTypes.Player)
				{
					message = "Не хватает патронов, внимательнее, мы пропустили ход!";
				}
				else
				{
					BuyAmmo();
					return;
				}
			}
			else
			{
				Ammo.Subtract(1);
				Random r = new Random();
				// 10%-ый шанс на критический выстрел
				if (0.1 > r.NextDouble())
				{
					currentDamage *= 1.2f;
					message = $"Произведён критический выстрел на {currentDamage} урона.\n";
				}
				// 20%-ый шанс на промах
				else if (0.2 > r.NextDouble())
				{
					currentDamage = 0;
					message = $"Произведён тактический промах с целью запугивания.\n";
				}
				Console.WriteLine(message);
				opponent.TakeDamage(currentDamage);
			}
		}
		// Метод получения урона с учётом брони
		public void TakeDamage(float damage)
		{
			// Броня сокращает урон вдвое
			if (Armor > 0 && damage != 0)
			{
				damage /= 2f;
				Armor--;
				Console.WriteLine("Урон снаряда был погашен наполовину.");
			}
			// Перевод Float -> Int приближает к ЧЁТНОМУ целому числу
			// Для лучшей точности добавлен Math.Round
			Health.Subtract(Convert.ToInt32(Math.Round(damage)));
		}

		public void Repair(int healthAmount)
		{
			Health.Add(healthAmount);
			if (Health.IsMax)
			{
				Console.WriteLine("Произведена 100 %-ная починка.");
			}
			else
			{
				Console.WriteLine($"Произведена починка на {healthAmount} единиц.");
			}
		}
		//Починка без конкретного количества восстанавливает 1/8 максимального здоровья
		public void Repair()
		{
			int healthAmount = (Health.Max / 8).Clamp(1, Health.Max);
			Repair(healthAmount);
		}

		public void BuyAmmo(int ammoAmount)
		{
			Ammo.Add(Math.Abs(ammoAmount));
			if (Ammo.IsMax)
				Console.WriteLine("Закуплено патронов до максимума.");
			else
				Console.WriteLine($"Закуплено {ammoAmount} патронов.");
		}
		// Закупка патронов без конкретного объёма закупает четверть от максимума
		// Или как минимум 1 патрон
		public void BuyAmmo()
		{
			int ammoAmount = (Ammo.Max / 4).Clamp(1, Ammo.Max);
			BuyAmmo(ammoAmount);
		}


		public void WriteInfo()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append($"Здоровье: {(int)Health}/{Health.Max}\n");
			if (Armor > 0)
				sb.Append($"Броня: {Armor}\n");
			sb.Append($"Количество патронов: {(int)Ammo}/{Ammo.Max}\n");

			Console.WriteLine(sb.ToString());
		}
		public Tank(TankTypes tankType, int armor, int damage, int ammo, int health)
		{
			TankType = tankType;
			Armor = armor;
			Damage = damage;
			Ammo = (NaturalNumber)ammo;
			Health = (NaturalNumber)health;
		}
	}



	class MyTank : ITank
	{
		public int Armor { get; set; }
		public float Damage { get; set; }
		public NaturalNumber Ammo { get; set; }
		public NaturalNumber Health { get; set; }

		int AmmoToBuy;
		int HealthToHeal;

		public void BuyAmmo(int ammoAmount)
		{
			//Подсчёт 
			int boughtAmmo = ammoAmount;
			if (ammoAmount + Ammo > Ammo.Max)
			{
				boughtAmmo = Ammo.Max - Ammo;
			}

			Ammo.Add(ammoAmount);
			Console.WriteLine($"Было закуплено {boughtAmmo} патронов.");
		}
		public void BuyAmmo()
		{
			BuyAmmo(AmmoToBuy);
		}
		public void Repair(int healthAmount)
		{
			int healedHealth = healthAmount;
			if (healthAmount + Health > Health.Max)
			{
				healedHealth = Health.Max - Health;
			}
			Health.Add(healthAmount);
			Console.WriteLine($"Произведена починка на {healedHealth} единиц.");
		}
		public void Repair()
		{
			Repair(HealthToHeal);
		}

		public void Shoot(ITank opponent)
		{
			if (Ammo.IsZero)
			{
				BuyAmmo();
				return;
			}
			else
			{
				Ammo.Subtract(1);
				Random r = new Random();
				double shootChance = r.NextDouble();
				float currentDamage = Damage;
				var message = "Был произведён выстрел.";
				// Шанс критического выстрела
				if (shootChance <= 0.1)
				{
					currentDamage *= 1.2f;
					message = "Был произведён критический выстрел.";
				}
				// Шанс промаха
				else if (shootChance <= 0.3)
				{
					currentDamage = 0;
					message = "Случился промах.";
				}
				Console.WriteLine(message);

				opponent.TakeDamage(currentDamage);

			}
		}

		public void TakeDamage(float damage)
		{
			if (damage == 0)
				return;

			var message = $"Было нанесено ";
			if (Armor > 0)
			{
				Armor--;
				damage *= 0.8f;
				message = $"Броня теряет прочность, было нанесено ";
			}

			int currentDamage = Convert.ToInt32(Math.Round(damage));
			Health.Subtract(currentDamage);

			message += $"{currentDamage} единиц урона.";
			Console.WriteLine(message);
		}

		public void WriteInfo()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append($"Здоровье: {(int)Health}/{Health.Max}\n");
			if (Armor > 0)
				sb.Append($"Броня: {Armor}\n");
			sb.Append($"Количество патронов: {(int)Ammo}/{Ammo.Max}\n");

			Console.WriteLine(sb.ToString());
		}

		public MyTank(int armor, int damage, int ammo, int health, int ammoToBuy, int healthToHeal)
		{
			Armor = armor;
			Damage = damage;
			Ammo = (NaturalNumber)ammo;
			Health = (NaturalNumber)health;
			AmmoToBuy = ammoToBuy;
			HealthToHeal = healthToHeal;
		}
	}
	
	class ComputerTank : MyTank, IComputer
	{
		public void ComputerTurn(ITank opponent)
		{
			if (Health.IsMax)
			{
				Shoot(opponent);
			}
			else
			{
				Random r = new Random();
				if (0.5f > r.NextDouble())
				{
					Shoot(opponent);
				}
				else
				{
					Repair();
				}
			}
		}

		public ComputerTank(int armor, int damage, int ammo, int health, int ammoToBuy, int healthToHeal) : base(armor, damage, ammo, health, ammoToBuy, healthToHeal)
		{ }
	}
	
	enum TankTypes
	{
		Player,
		Enemy
	}
	
	/// <summary>
	/// Класс целочисленного значения, ограниченного нулём и максимумом
	/// </summary>
	class NaturalNumber
	{
		int Num;
		readonly int MaxNum;

		public void Add(int num)
		{
			Num = (Num + num).Clamp(0, MaxNum);
		}
		public void Subtract(int num)
		{
			Num = (Num - num).Clamp(0, MaxNum);
		}
		public int Max => MaxNum;
		public bool IsMax => Num == MaxNum;
		public bool IsZero => Num == 0;

		public NaturalNumber(int num, int maxNum)
		{
			Num = num;
			MaxNum = maxNum;
		}
		public NaturalNumber(int num) : this(num, num)
		{ }

		public static implicit operator int(NaturalNumber number)
		{
			return number.Num;
		}
		public static explicit operator NaturalNumber(int number)
		{
			return new NaturalNumber(number);
		}
	}



	// Расширение функциональности int
	// Добавленная функция Clamp используется в NaturalNumber
	public static partial class ExtensionMethods
	{
		public static int Clamp(this int value, int min, int max)
		{
			return (value < min) ? min : (value > max) ? max : value;
		}
	}

	class Program
	{
		static void ShowInfoAndActions(ITank player, ITank enemy)
		{
			Console.WriteLine("Состояние игрока:");
			player.WriteInfo();
			Console.WriteLine("Состояние врага:");
			enemy.WriteInfo();

			Console.WriteLine("[НАШ ХОД]\n" +
				"Действия (нажмите соответствующую клавишу):\n" +
				"1. Выстрел.\n" +
				"2. Починка.\n" +
				"3. Закупка патронов.\n");
		}
		static void EnemyMove(Tank enemy, Tank player)
		{
			
			if (enemy.Health.IsMax)
			{
				enemy.Shoot(player);
			}
			else
			{
				Random r = new Random();
				if (0.5f > r.NextDouble())
				{
					enemy.Shoot(player);
				}
				else
				{
					enemy.Repair();
				}
			}
		}


		static void Main(string[] args)
		{
			// Создание объектов игрока и врага 
			MyTank player = new MyTank(5, 15, 10, 50, 3, 10);
			ComputerTank enemy = new ComputerTank(2, 10, 5, 100, 5, 12);

			Console.WriteLine("Добро пожаловать на танковый полигон, стреляйте первым!");

			// Главный цикл игры
			while (!player.Health.IsZero && !enemy.Health.IsZero)
			{
				ShowInfoAndActions(player, enemy);

				// Считывание нажатой клавиши игрока
				char playerInput = Console.ReadKey(true).KeyChar;
				
				// Недопуск неверного ввода
				while (playerInput < '1' || playerInput > '3')
				{
					Console.WriteLine("Попробуйте нажать те циферки сверху! У вас получится!");
					playerInput = Console.ReadKey(true).KeyChar;
				}
				switch(playerInput)
				{
					case '1':
						player.Shoot(enemy); 
						break;
					case '2':
						player.Repair();
						break;
					case '3':
						player.BuyAmmo();
						break;
				}
				Console.WriteLine("[ХОД СУПОСТАТА]");
				enemy.ComputerTurn(player);
				Console.WriteLine("Нажмите любую клавишу, чтобы продолжить.");
				Console.ReadKey(true);
				Console.Clear();
			}
			if (player.Health.IsZero && enemy.Health.IsZero)
			{
				Console.WriteLine("Случай сыграл с Вами в злую шутку, ничья...");
			}
			else if (player.Health.IsZero)
			{
				Console.WriteLine("К сожалению, Вы проиграли...");
			}
			else
			{
				Console.WriteLine("Фух, победа! Можете отдохнуть!");
			}
			Console.ReadLine();
		}
	}
}
