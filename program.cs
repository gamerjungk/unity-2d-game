// See https://aka.ms/new-console-template for more information
using System;

class Program
{
    static void Main()
    {
        Random random = new Random();
        int targetNumber = random.Next(1, 101); // 1부터 100 사이의 랜덤 숫자
        int guess = 0;
        int attempts = 0;

        Console.WriteLine("숫자 맞추기 게임입니다. 1부터 100 사이의 숫자를 맞춰보세요!");

        while (guess != targetNumber)
        {
            Console.Write("숫자를 입력하세요: ");
            string input = Console.ReadLine();
            
            if (int.TryParse(input, out guess))
            {
                attempts++;

                if (guess < targetNumber)
                {
                    Console.WriteLine("너무 낮습니다! 다시 시도하세요.");
                }
                else if (guess > targetNumber)
                {
                    Console.WriteLine("너무 높습니다! 다시 시도하세요.");
                }
                else
                {
                    Console.WriteLine($"정답입니다! {attempts}번 컷!");
                }
            }
            else
            {
                Console.WriteLine("유효한 숫자를 입력하세요.");
            }
        }
    }
}
