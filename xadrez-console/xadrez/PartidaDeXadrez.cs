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
        public Peca _vulneravelEnPassant { get; private set; }
        public bool xeque = false;
        private HashSet<Peca> pecas;
        private HashSet<Peca> capturadas;

        public PartidaDeXadrez()
        {
            Tab = new Tabuleiro(8, 8);
            turno = 1;
            jogadorAtual = Cor.Branca;
            Terminada = false;
            xeque = false;
            _vulneravelEnPassant = null;
            pecas = new HashSet<Peca>();
            capturadas = new HashSet<Peca>();
            ColocarPecas();
        }

        public Peca ExecutaMovimento(Posicao origem, Posicao destino)
        {
            Peca p = Tab.RetirarPeca(origem);
            p.IncrementarMovimentos();
            Peca pecaCapturada = Tab.RetirarPeca(destino);
            Tab.ColocarPeca(p, destino);
            if (pecaCapturada != null)
                capturadas.Add(pecaCapturada);

            if (p is Rei && destino.coluna == origem.coluna + 2)
            {
                Posicao origemTorre = new Posicao(origem.linha, origem.coluna + 3);
                Posicao destinoTorre = new Posicao(origem.linha, origem.coluna + 1);
                Peca T = Tab.RetirarPeca(origemTorre);
                T.IncrementarMovimentos();
                Tab.ColocarPeca(T, destinoTorre);
            }

            if (p is Rei && destino.coluna == origem.coluna - 2)
            {
                Posicao origemTorre = new Posicao(origem.linha, origem.coluna - 4);
                Posicao destinoTorre = new Posicao(origem.linha, origem.coluna - 1);
                Peca T = Tab.RetirarPeca(origemTorre);
                T.IncrementarMovimentos();
                Tab.ColocarPeca(T, destinoTorre);
            }

            if (p is Peao)
            {
                if (origem.coluna != destino.coluna  && pecaCapturada == null)
                {
                    Posicao posP;
                    if (p.cor == Cor.Branca)
                        posP = new Posicao(destino.linha + 1, destino.coluna);
                    else
                        posP = new Posicao(destino.linha - 1, destino.coluna);
                    pecaCapturada = Tab.RetirarPeca(posP);
                    capturadas.Add(pecaCapturada);
                }
            }
            return pecaCapturada;
        }

        public void RealizaJogada(Posicao origem, Posicao destino)
        {
            Peca pecaCapturada = ExecutaMovimento(origem, destino);
            Peca p = Tab.Peca(destino);

            if (p is Peao)
                if ((p.cor == Cor.Branca && destino.linha == 0) || (p.cor == Cor.Preta && destino.linha == 7))
                {
                    p = Tab.RetirarPeca(destino);
                    pecas.Remove(p);
                    Peca dama = new Queen(tab:, p.cor);
                    pecas.Add(dama);
                }

            if (EstaEmXeque(jogadorAtual))
            {
                DesfazMovimento(origem, destino, pecaCapturada);
                throw new TabuleiroException("Você não pode se colocar em xeque!");
            }

            if (EstaEmXeque(Adversaria(jogadorAtual)))
                xeque = true;
            else
                xeque = false;

            if (TesteXequemate(Adversaria(jogadorAtual)))
                Terminada = true;
            else
            {
                turno++;
                MudaJogador();
            }

            Peca p = Tab.Peca(destino);
            if (p is Peao && (destino.linha == origem.linha - 2 || destino.linha == origem.linha + 2))
                _vulneravelEnPassant = p;
        }

        public void DesfazMovimento(Posicao origem, Posicao destino, Peca pecaCapturada)
        {
            Peca p = Tab.RetirarPeca(destino);
            p.DecrementarMovimentos();
            if (pecaCapturada != null)
            {
                Tab.ColocarPeca(pecaCapturada, destino);
                capturadas.Remove(pecaCapturada);
            }
            Tab.ColocarPeca(p, origem);

            if (p is Rei && destino.coluna == origem.coluna + 2)
            {
                Posicao origemTorre = new Posicao(origem.linha, origem.coluna + 3);
                Posicao destinoTorre = new Posicao(origem.linha, origem.coluna + 1);
                Peca T = Tab.RetirarPeca(destinoTorre);
                T.DecrementarMovimentos();
                Tab.ColocarPeca(T, origemTorre);
            }

            if (p is Rei && destino.coluna == origem.coluna - 2)
            {
                Posicao origemTorre = new Posicao(origem.linha, origem.coluna - 4);
                Posicao destinoTorre = new Posicao(origem.linha, origem.coluna - 1);
                Peca T = Tab.RetirarPeca(destinoTorre);
                T.DecrementarMovimentos();
                Tab.ColocarPeca(T, origemTorre);
            }

            if (p is Peao)
            {
                if (origem.coluna != destino.coluna && pecaCapturada == _vulneravelEnPassant)
                {
                    Peca peao = Tab.RetirarPeca(destino);
                    Posicao posP;
                    if (p.cor == Cor.Branca)
                        posP = new Posicao(3, destino.coluna);
                    else
                        posP = new Posicao(4, destino.coluna);
                }                    
            }
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
            if (!Tab.Peca(origem).MovimentoPossivel(destino))
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

        private Cor Adversaria(Cor cor)
        {
            if (cor == Cor.Branca)
                return Cor.Preta;
            else
                return Cor.Branca;
        }

        private Peca Rei(Cor cor)
        {
            foreach (Peca x in PecasEmJogo(cor))
            {
                if (x is Rei)
                    return x;
            }
            return null;
        }

        public bool EstaEmXeque(Cor cor)
        {
            Peca R = Rei(cor);

            if (R == null)
                throw new TabuleiroException("Não tem rei da cor " + cor + " no tabuleiro");

            foreach (Peca x in PecasEmJogo(Adversaria(cor)))
            {
                bool[,] mat = x.MovimentosPossiveis();
                if (mat[R.posicao.linha, R.posicao.coluna])
                    return true;
            }
            return false;
        }

        public bool TesteXequemate(Cor cor)
        {
            if (!EstaEmXeque(cor))
                return false;
            foreach (Peca x in PecasEmJogo(cor))
            {
                bool[,] mat = x.MovimentosPossiveis();
                for (int i = 0; i < Tab.linhas; i++)
                {
                    for (int j = 0; j < Tab.colunas; j++)
                    {
                        if (mat[i, j])
                        {
                            Posicao origem = x.posicao;
                            Posicao destino = new Posicao(i, j);
                            Peca pecaCapturada = ExecutaMovimento(origem, destino);
                            bool testeXeque = EstaEmXeque(cor);
                            DesfazMovimento(origem, destino, pecaCapturada);
                            if (!testeXeque)
                                return false;
                        }
                    }
                }
            }
            return true;
        }

        public void ColocarNovapeca(char coluna, int linha, Peca peca)
        {
            Tab.ColocarPeca(peca, new PosicaoXadrez(coluna, linha).ToPosicao());
            pecas.Add(peca);
        }

        private void ColocarPecas()
        {
            ColocarNovapeca('a', 1, new Torre(Tab, Cor.Branca));
            ColocarNovapeca('b', 1, new Cavalo(Tab, Cor.Branca));
            ColocarNovapeca('c', 1, new Bispo(Tab, Cor.Branca));
            ColocarNovapeca('d', 1, new Queen(Tab, Cor.Branca));
            ColocarNovapeca('e', 1, new Rei(Tab, Cor.Branca, this));
            ColocarNovapeca('f', 1, new Bispo(Tab, Cor.Branca));
            ColocarNovapeca('g', 1, new Cavalo(Tab, Cor.Branca));
            ColocarNovapeca('h', 1, new Torre(Tab, Cor.Branca));
            ColocarNovapeca('a', 2, new Peao(Tab, Cor.Branca, this));
            ColocarNovapeca('b', 2, new Peao(Tab, Cor.Branca, this));
            ColocarNovapeca('c', 2, new Peao(Tab, Cor.Branca, this));
            ColocarNovapeca('d', 2, new Peao(Tab, Cor.Branca, this));
            ColocarNovapeca('e', 2, new Peao(Tab, Cor.Branca, this));
            ColocarNovapeca('f', 2, new Peao(Tab, Cor.Branca, this));
            ColocarNovapeca('g', 2, new Peao(Tab, Cor.Branca, this));
            ColocarNovapeca('h', 2, new Peao(Tab, Cor.Branca, this));

            ColocarNovapeca('a', 8, new Torre(Tab, Cor.Preta));
            ColocarNovapeca('b', 8, new Cavalo(Tab, Cor.Preta));
            ColocarNovapeca('c', 8, new Bispo(Tab, Cor.Preta));
            ColocarNovapeca('d', 8, new Queen(Tab, Cor.Preta));
            ColocarNovapeca('e', 8, new Rei(Tab, Cor.Preta, this));
            ColocarNovapeca('f', 8, new Bispo(Tab, Cor.Preta));
            ColocarNovapeca('g', 8, new Cavalo(Tab, Cor.Preta));
            ColocarNovapeca('h', 8, new Torre(Tab, Cor.Preta));
            ColocarNovapeca('a', 7, new Peao(Tab, Cor.Preta, this));
            ColocarNovapeca('b', 7, new Peao(Tab, Cor.Preta, this));
            ColocarNovapeca('c', 7, new Peao(Tab, Cor.Preta, this));
            ColocarNovapeca('d', 7, new Peao(Tab, Cor.Preta, this));
            ColocarNovapeca('e', 7, new Peao(Tab, Cor.Preta, this));
            ColocarNovapeca('f', 7, new Peao(Tab, Cor.Preta, this));
            ColocarNovapeca('g', 7, new Peao(Tab, Cor.Preta, this));
            ColocarNovapeca('h', 7, new Peao(Tab, Cor.Preta, this));
        }
    }
}