﻿using System;
using tabuleiro;
using System.Collections.Generic;

namespace xadrez
{
    class PartidaDeXadrez
    {
        public Tabuleiro Tab { get; private set; }
        public int turno { get; private set; }
        public Cor jogadorAtual { get; private set; }
        public bool Terminada { get; private set; }
        private HashSet<Peca> pecas;
        private HashSet<Peca> capturadas;

        public PartidaDeXadrez()
        {
            Tab = new Tabuleiro(8, 8);
            turno = 1;
            jogadorAtual = Cor.Branca;
            pecas = new HashSet<Peca>();
            capturadas = new HashSet<Peca>();
            ColocarPecas();
        }

        public void ExecutaMovimento(Posicao origem, Posicao destino)
        {
            Peca p = Tab.RetirarPeca(origem);
            p.IncrementarMovimentos();
            Peca pecaCapturada = Tab.RetirarPeca(destino);
            Tab.ColocarPeca(p, destino);
            if (pecaCapturada != null)
                capturadas.Add(pecaCapturada);
        }

        public void RealizaJogada(Posicao origem, Posicao destino)
        {
            ExecutaMovimento(origem, destino);
            turno++;
            MudaJogador();
        }

        public void ValidarPosicaoOrigem(Posicao pos)
        {
            if (Tab.Peca(pos) == null)
                throw new TabuleiroException("Não existe peça na posição de origem escolhida!");
            if (jogadorAtual != Tab.Peca(pos).cor)
                throw new TabuleiroException("A peça de origem escolhida não é sua!");
            if (!Tab.Peca(pos).ExisteMovimentosPossiveis())
                throw new TabuleiroException("Não há movimentos possíveis para a peça de origem escolhida!");
        }

        public void ValidarPosicaoDestino(Posicao origem, Posicao destino)
        {
            if(!Tab.Peca(origem).PodeMoverPara(destino))
                throw new TabuleiroException("Posição de destino inválida");
        }

        private void MudaJogador()
        {
            if (jogadorAtual == Cor.Branca)
                jogadorAtual = Cor.Preta;
            else
                jogadorAtual = Cor.Branca;
        }

        public HashSet<Peca> PecasCapturadas(Cor cor)
        {
            HashSet<Peca> aux = new HashSet<Peca>();
            foreach (Peca x in capturadas)
            {
                if (x.cor == cor)
                    aux.Add(x);
            }
            return aux;
        }

        public HashSet<Peca> PecasEmJogo(Cor cor)
        {
            HashSet<Peca> aux = new HashSet<Peca>();
            foreach (Peca x in pecas)
            {
                if (x.cor == cor)
                    aux.Add(x);
            }
            aux.ExceptWith(PecasCapturadas(cor));
            return aux;
        }

        public void ColocarNovapeca(char coluna, int linha, Peca peca)
        {
            Tab.ColocarPeca(peca, new PosicaoXadrez(coluna, linha).ToPosicao());
            pecas.Add(peca);
        }

        private void ColocarPecas()
        {
            ColocarNovapeca('c', 1, new Torre(Tab, Cor.Branca));
            ColocarNovapeca('c', 2, new Torre(Tab, Cor.Branca));
            ColocarNovapeca('d', 2, new Torre(Tab, Cor.Branca));
            ColocarNovapeca('e', 2, new Torre(Tab, Cor.Branca));
            ColocarNovapeca('e', 1, new Torre(Tab, Cor.Branca));
            ColocarNovapeca('d', 1, new Rei(Tab, Cor.Branca));

            ColocarNovapeca('c', 7, new Torre(Tab, Cor.Preta));
            ColocarNovapeca('c', 8, new Torre(Tab, Cor.Preta));
            ColocarNovapeca('d', 7, new Torre(Tab, Cor.Preta));
            ColocarNovapeca('e', 7, new Torre(Tab, Cor.Preta));
            ColocarNovapeca('e', 8, new Torre(Tab, Cor.Preta));
            ColocarNovapeca('d', 8, new Rei(Tab, Cor.Preta));
        }
    }
}