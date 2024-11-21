using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace MemoryGame
{
    public partial class MainWindow : Window
    {
        private List<Button> cardButtons;
        private Dictionary<Button, string> cardPairs;
        private Button firstCard;
        private Button secondCard;
        private DispatcherTimer timer;
        private DispatcherTimer gameTimer;
        private int elapsedTime;

        public MainWindow()
        {
            InitializeComponent();
            SetupGame();
        }

        private void SetupGame()
        {
            
            cardButtons = new List<Button>();
            cardPairs = new Dictionary<Button, string>();
            firstCard = null;
            secondCard = null;
            elapsedTime = 0;

           
            gameTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            gameTimer.Tick += (s, e) => TimerText.Text = $"Eltelt idő: {++elapsedTime} mp";

            
            timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            timer.Tick += FlipBackCards;

            
            NewGame();
        }

        private void NewGameButton_Click(object sender, RoutedEventArgs e) => NewGame();

        private void NewGame()
        {
            
            timer.Stop();
            gameTimer.Stop();

            
            CardGrid.Children.Clear();
            cardButtons.Clear();
            cardPairs.Clear();
            firstCard = null;
            secondCard = null;
            elapsedTime = 0;
            TimerText.Text = "Eltelt idő: 0 sec";
            gameTimer.Start();

            
            var pairs = GeneratePairs();
            pairs = pairs.OrderBy(a => Guid.NewGuid()).ToList();

            
            foreach (var image in pairs)
            {
                var button = new Button { Content = "?", FontSize = 24 };
                button.Click += Card_Click;
                cardButtons.Add(button);
                cardPairs[button] = image;  
                CardGrid.Children.Add(button);
            }
        }

        private List<string> GeneratePairs()
        {
            
            return Enumerable.Range(1, 8).Select(i => i.ToString()).Concat(Enumerable.Range(1, 8).Select(i => i.ToString())).ToList();
        }

        private void Card_Click(object sender, RoutedEventArgs e)
        {
            if (timer.IsEnabled) return;

            var clickedCard = sender as Button;

            if (firstCard == null)
            {
                firstCard = clickedCard;
                RevealCard(firstCard);
            }
            else if (secondCard == null && clickedCard != firstCard)
            {
                secondCard = clickedCard;
                RevealCard(secondCard);

                if (cardPairs[firstCard] == cardPairs[secondCard])
                {
                    firstCard = null;
                    secondCard = null;
                    CheckGameEnd();
                }
                else
                {
                    timer.Start();
                }
            }
        }

        private void RevealCard(Button card) => card.Content = cardPairs[card];

        private void FlipBackCards(object sender, EventArgs e)
        {
            timer.Stop();
            firstCard.Content = "?";
            secondCard.Content = "?";
            firstCard = null;
            secondCard = null;
        }

        private void CheckGameEnd()
        {
            if (cardButtons.All(b => b.Content.ToString() != "?"))
            {
                gameTimer.Stop();
                MessageBox.Show($"Gratulálunk, megnyerted a játékot! Eltelt idő: {elapsedTime} mp");
            }
        }
    }
}
