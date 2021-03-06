﻿namespace Chess.Site.Domain
{
    using System;
    using Models;
    using static System.Math;

    public class GameResult
    {
        public static readonly int StartDecipoints = 1000 * 100;

        public int WhiteDeltaDecipoints { get; private set; }
        public decimal WhiteDeltaPoints => WhiteDeltaDecipoints / 100m;

        public int BlackDeltaDecipoints { get; private set; }
        public decimal BlackDeltaPoints => BlackDeltaDecipoints / 100m;

        public int WhitePlayerId { get; private set; }
        public int BlackPlayerId { get; private set; }

        public Winner Winner { get; private set; }
        public string WinnerText => Winner.ToString();

        public DateTime CreatedAt { get; set; }

        public GameResult(Player whitePlayer, Player blackPlayer, Winner winner)
        {
            WhitePlayerId = whitePlayer.Id;
            BlackPlayerId = blackPlayer.Id;
            Winner = winner;
            CreatedAt = DateTime.UtcNow;

            Update(whitePlayer, blackPlayer, winner == Winner.White, winner == Winner.Black);
        }

        //for dapper
        protected GameResult()
        {
        }



        private void Update(Player ra, Player rb, bool raWin, bool rbWin)
        {
            WhiteDeltaDecipoints = Calc(ra.Decipoints, rb.Decipoints, GetWinPoints(raWin, rbWin));
            BlackDeltaDecipoints = Calc(rb.Decipoints, ra.Decipoints, GetWinPoints(rbWin, raWin));

            ra.Decipoints += WhiteDeltaDecipoints;
            rb.Decipoints += BlackDeltaDecipoints;
        }

        private static decimal GetWinPoints(bool raWin, bool rbWin)
        {
            if (raWin == rbWin)
                return 0.5m;

            if (raWin)
                return 1m;

            return 0m;
        }

        const decimal K = 16m;
        private static int Calc(int ra, int rb, decimal sa)
        {
            decimal dra = ra / 100m;
            decimal drb = rb / 100m;

            decimal ea = 1 / (1 + (decimal)Pow(10, (double)(drb - dra) / 400d));

            return (int)Round(K * (sa - ea) * 100m, 0);
        }

        public bool WithPlayer(int playerId)
        {
            return WhitePlayerId == playerId || BlackPlayerId == playerId;
        }

        public decimal GetPlayerScore(int playerId)
        {
            if (WhitePlayerId == playerId)
            {
                switch (Winner)
                {
                    case Winner.White:
                        return 1;
                    case Winner.Black:
                        return 0;
                    case Winner.Nobody:
                        return 0.5m;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            if (BlackPlayerId == playerId)
            {
                switch (Winner)
                {
                    case Winner.White:
                        return 0;
                    case Winner.Black:
                        return 1;
                    case Winner.Nobody:
                        return 0.5m;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            throw new ArgumentOutOfRangeException("Игрок не принимал участия в партии");

        }
    }
}
