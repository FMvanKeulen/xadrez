namespace tabuleiro
{
    class Posicao
    {
        public int linha;
        public int coluna;

        public Posicao(int linha, int coluna)
        {
            this.linha = linha;
            this.coluna = coluna;
        }

        public void DefinirValores(int linha, int coluna)
        {
            this.linha = linha;
            this.coluna = coluna;
        }

        public override string ToString()
        {
            return linha 
                + ", " 
                + coluna;
        }
    }
}