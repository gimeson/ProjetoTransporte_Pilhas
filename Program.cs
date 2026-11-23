using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace ControleFretamentoAeroportos
{
    public class Pilha<T>
    {
        private readonly List<T> _elementos = new List<T>();

        public void Push(T item)
        {
            _elementos.Add(item);
        }

        public T Pop()
        {
            if (IsEmpty())
                throw new InvalidOperationException("A pilha está vazia.");

            int topoIndex = _elementos.Count - 1;
            T item = _elementos[topoIndex];
            _elementos.RemoveAt(topoIndex);
            return item;
        }

        public T Peek()
        {
            if (IsEmpty())
                throw new InvalidOperationException("A pilha está vazia.");

            return _elementos[_elementos.Count - 1];
        }

        public bool IsEmpty() => _elementos.Count == 0;

        public int Size() => _elementos.Count;

        // Retorna do topo (próximo a sair) para a base
        public List<T> Listar()
        {
            var copia = new List<T>(_elementos);
            copia.Reverse();
            return copia;
        }
    }

    public class Garagem
    {
        public string Nome { get; private set; }
        public int CapacidadeMaxima { get; private set; }
        public Pilha<Veiculo> Veiculos { get; private set; }

        public Garagem(string nome, int capacidadeMaxima = 20)
        {
            if (string.IsNullOrWhiteSpace(nome))
                throw new ArgumentException("Nome da garagem é obrigatório.");
            if (capacidadeMaxima <= 0)
                throw new ArgumentException("Capacidade máxima deve ser maior que zero.");

            Nome = nome.Trim();
            CapacidadeMaxima = capacidadeMaxima;
            Veiculos = new Pilha<Veiculo>();
        }

        public bool EstaVazia() => Veiculos.IsEmpty();

        public bool EstaCheia() => Veiculos.Size() >= CapacidadeMaxima;

        public void EmpilharVeiculo(Veiculo v)
        {
            if (v == null)
                throw new ArgumentNullException(nameof(v));
            if (EstaCheia())
                throw new InvalidOperationException($"A garagem '{Nome}' atingiu sua capacidade máxima.");

            Veiculos.Push(v);
            v.SetGaragemAtual(this);
        }

        public Veiculo DesempilharVeiculo()
        {
            if (EstaVazia())
                throw new InvalidOperationException($"A garagem '{Nome}' está vazia.");

            Veiculo v = Veiculos.Pop();
            v.SetGaragemAtual(null);
            return v;
        }

        public int PotencialTransporte()
        {
            return Veiculos.Listar().Sum(v => v.CapacidadePassageiros);
        }

        public List<Veiculo> ListarVeiculos() => Veiculos.Listar();

        public override string ToString() => Nome;
    }

    public class Veiculo
    {
        public int Numero { get; private set; }
        public int CapacidadePassageiros { get; private set; }
        public Garagem GaragemAtual { get; private set; }

        public Veiculo(int numero, int capacidadePassageiros)
        {
            if (numero <= 0)
                throw new ArgumentException("Número do veículo deve ser maior que zero.");
            if (capacidadePassageiros <= 0)
                throw new ArgumentException("Capacidade de passageiros deve ser maior que zero.");

            Numero = numero;
            CapacidadePassageiros = capacidadePassageiros;
        }

        public int GetNumero() => Numero;
        public int GetCapacidadePassageiros() => CapacidadePassageiros;
        public Garagem GetGaragemAtual() => GaragemAtual;
        public void SetGaragemAtual(Garagem g) => GaragemAtual = g;

        public override string ToString() => $"Van #{Numero} (Lotação: {CapacidadePassageiros})";
    }

    public class DistribuicaoInicial
    {
        public Veiculo Veiculo { get; private set; }
        public Garagem GaragemInicial { get; private set; }

        public DistribuicaoInicial(Veiculo veiculo, Garagem garagemInicial)
        {
            Veiculo = veiculo;
            GaragemInicial = garagemInicial;
        }

        public Veiculo GetVeiculo() => Veiculo;
        public Garagem GetGaragemInicial() => GaragemInicial;
    }

    public class Viagem
    {
        public Garagem Origem { get; private set; }
        public Garagem Destino { get; private set; }
        public Veiculo Veiculo { get; private set; }
        public int QuantidadePassageiros { get; private set; }
        public DateTime HorarioSaida { get; private set; }
        public DateTime? HorarioChegada { get; private set; }

        public Viagem(Garagem origem, Garagem destino, Veiculo veiculo, DateTime horarioSaida)
        {
            Origem = origem;
            Destino = destino;
            Veiculo = veiculo;
            HorarioSaida = horarioSaida;
        }

        public void RegistrarPassageiros(int qtd)
        {
            if (qtd < 0 || qtd > Veiculo.CapacidadePassageiros)
                throw new ArgumentOutOfRangeException(nameof(qtd), "Quantidade de passageiros inválida para o veículo.");

            QuantidadePassageiros = qtd;
        }

        public void RegistrarChegada(DateTime? horarioChegada = null)
        {
            HorarioChegada = horarioChegada ?? DateTime.Now;
        }

        public override string ToString()
        {
            string chegadaStr = HorarioChegada.HasValue
                ? HorarioChegada.Value.ToString("HH:mm:ss")
                : "Em rota";

            return $"[{HorarioSaida:HH:mm:ss} -> {chegadaStr}] {Veiculo} | {Origem.Nome} -> {Destino.Nome} | Passageiros: {QuantidadePassageiros}";
        }
    }

    public class RelatorioJornada
    {
        public Veiculo Veiculo { get; private set; }
        public int TotalPassageiros { get; private set; }

        public RelatorioJornada(Veiculo veiculo, int totalPassageiros)
        {
            Veiculo = veiculo;
            TotalPassageiros = totalPassageiros;
        }

        public Veiculo GetVeiculo() => Veiculo;
        public int GetTotalPassageiros() => TotalPassageiros;
    }

    public class Jornada
    {
        public DateTime Data { get; private set; }
        public bool Iniciada { get; private set; }
        public bool Encerrada { get; private set; }
        public List<Viagem> Viagens { get; private set; }
        public List<DistribuicaoInicial> DistribuicaoInicial { get; private set; }

        public Jornada(DateTime data)
        {
            Data = data;
            Iniciada = false;
            Encerrada = false;
            Viagens = new List<Viagem>();
            DistribuicaoInicial = new List<DistribuicaoInicial>();
        }

        public void Iniciar()
        {
            Iniciada = true;
            Encerrada = false;
            Viagens.Clear();
            DistribuicaoInicial.Clear();
        }

        public void RegistrarDistribuicao(DistribuicaoInicial d)
        {
            DistribuicaoInicial.Add(d);
        }

        public void RegistrarViagem(Viagem v)
        {
            Viagens.Add(v);
        }

        public List<RelatorioJornada> GerarRelatorioPorVeiculo(List<Veiculo> frota)
        {
            return frota
                .OrderBy(v => v.Numero)
                .Select(v => new RelatorioJornada(
                    v,
                    Viagens.Where(vg => vg.Veiculo.Numero == v.Numero).Sum(vg => vg.QuantidadePassageiros)
                ))
                .ToList();
        }

        public List<string> GerarRelatorioFinal(List<Veiculo> frota)
        {
            var relatorio = GerarRelatorioPorVeiculo(frota);
            var linhas = new List<string>
            {
                $"=== RELATÓRIO DE ENCERRAMENTO DE JORNADA ({Data:dd/MM/yyyy}) ==="
            };

            foreach (var item in relatorio)
            {
                linhas.Add($"Van #{item.Veiculo.Numero} (Lotação {item.Veiculo.CapacidadePassageiros}): {item.TotalPassageiros} passageiro(s) transportado(s)");
            }

            linhas.Add($"Total geral de passageiros no dia: {relatorio.Sum(r => r.TotalPassageiros)}");
            linhas.Add($"Total de viagens realizadas no dia: {Viagens.Count}");

            return linhas;
        }

        public void Encerrar()
        {
            Iniciada = false;
            Encerrada = true;
            Viagens.Clear();
            DistribuicaoInicial.Clear();
        }
    }

    public class Empresa
    {
        public string Nome { get; private set; }
        public bool JornadaIniciada => JornadaAtual != null && JornadaAtual.Iniciada;
        public bool JornadaEncerrada => JornadaAtual == null || JornadaAtual.Encerrada;
        public List<Veiculo> Veiculos { get; private set; }
        public List<Garagem> Garagens { get; private set; }
        public Jornada JornadaAtual { get; private set; }

        public Empresa(string nome)
        {
            Nome = nome;
            Veiculos = new List<Veiculo>();
            Garagens = new List<Garagem>();
        }

        public void CadastrarVeiculo(Veiculo v)
        {
            if (JornadaIniciada)
                throw new InvalidOperationException("Não é permitido cadastrar veículos com a jornada diária iniciada. Encerre a jornada primeiro.");

            if (Veiculos.Any(x => x.Numero == v.Numero))
                throw new InvalidOperationException($"Já existe um veículo cadastrado com o número {v.Numero}.");

            Veiculos.Add(v);
        }

        public void CadastrarGaragem(Garagem g)
        {
            if (JornadaIniciada)
                throw new InvalidOperationException("Não é permitido cadastrar garagens com a jornada diária iniciada. Encerre a jornada primeiro.");

            if (Garagens.Any(x => x.Nome.Equals(g.Nome, StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException($"Já existe uma garagem cadastrada com o nome '{g.Nome}'.");

            Garagens.Add(g);
        }

        public void IniciarJornada()
        {
            if (JornadaIniciada)
                throw new InvalidOperationException("A jornada diária já foi iniciada.");

            if (Garagens.Count < 2)
                throw new InvalidOperationException("É necessário ter ao menos 2 garagens cadastradas para iniciar a jornada.");

            if (Veiculos.Count == 0)
                throw new InvalidOperationException("É necessário ter ao menos 1 veículo cadastrado para iniciar a jornada.");

            // Limpa garagens antes de distribuir
            foreach (var g in Garagens)
            {
                while (!g.EstaVazia())
                    g.DesempilharVeiculo();
            }

            JornadaAtual = new Jornada(DateTime.Today);
            JornadaAtual.Iniciar();
            DistribuirVeiculos();
        }

        private void DistribuirVeiculos()
        {
            var frotaOrdenada = Veiculos.OrderBy(v => v.Numero).ToList();
            int indiceGaragem = 0;
            int totalGaragens = Garagens.Count;

            foreach (var veiculo in frotaOrdenada)
            {
                bool alocado = false;
                int tentativas = 0;

                while (!alocado && tentativas < totalGaragens)
                {
                    Garagem garagemAlvo = Garagens[indiceGaragem];
                    indiceGaragem = (indiceGaragem + 1) % totalGaragens;
                    tentativas++;

                    if (!garagemAlvo.EstaCheia())
                    {
                        garagemAlvo.EmpilharVeiculo(veiculo);
                        JornadaAtual.RegistrarDistribuicao(new DistribuicaoInicial(veiculo, garagemAlvo));
                        alocado = true;
                    }
                }

                if (!alocado)
                    throw new InvalidOperationException($"Não há vaga disponível nas garagens para alocar a Van #{veiculo.Numero}.");
            }
        }

        public List<string> EncerrarJornada()
        {
            if (!JornadaIniciada || JornadaAtual == null)
                throw new InvalidOperationException("Não há uma jornada iniciada para encerrar.");

            List<string> relatorio = JornadaAtual.GerarRelatorioFinal(Veiculos);
            JornadaAtual.Encerrar();
            return relatorio;
        }

        public Viagem LiberaViagem(Garagem origem, Garagem destino)
        {
            if (!JornadaIniciada || JornadaAtual == null)
                throw new InvalidOperationException("A viagem só pode ser liberada após o início da jornada diária.");

            if (origem == null || destino == null)
                throw new ArgumentException("Origem e destino devem ser informados.");

            if (origem == destino)
                throw new InvalidOperationException("A garagem de origem deve ser diferente da garagem de destino.");

            if (origem.EstaVazia())
                throw new InvalidOperationException($"Garagem '{origem.Nome}' está vazia. Aguarde o retorno de um veículo para liberar nova viagem.");

            if (destino.EstaCheia())
                throw new InvalidOperationException($"Garagem '{destino.Nome}' está sem vagas disponíveis no momento.");

            Veiculo veiculo = origem.DesempilharVeiculo();
            DateTime saida = DateTime.Now;

            var viagem = new Viagem(origem, destino, veiculo, saida);
            // Lotação completa do veículo ao sair
            viagem.RegistrarPassageiros(veiculo.CapacidadePassageiros);
            // Chegada ao destino e estacionamento na garagem de destino
            viagem.RegistrarChegada(saida.AddMinutes(35));
            destino.EmpilharVeiculo(veiculo);

            JornadaAtual.RegistrarViagem(viagem);
            return viagem;
        }

        public List<Veiculo> ListarVeiculos(Garagem garagem)
        {
            if (garagem == null)
                throw new ArgumentNullException(nameof(garagem));

            return garagem.ListarVeiculos();
        }

        public List<Viagem> ConsultarViagens(Garagem origem, Garagem destino)
        {
            if (JornadaAtual == null)
                return new List<Viagem>();

            return JornadaAtual.Viagens
                .Where(v => v.Origem == origem && v.Destino == destino)
                .ToList();
        }

        public int ConsultarPassageiros(Garagem origem, Garagem destino)
        {
            return ConsultarViagens(origem, destino).Sum(v => v.QuantidadePassageiros);
        }

        public Garagem BuscarGaragemPorNome(string nome)
        {
            return Garagens.FirstOrDefault(g => g.Nome.Equals(nome?.Trim(), StringComparison.OrdinalIgnoreCase));
        }
    }

    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Empresa empresa = new Empresa("AeroFretamento Express");
            InicializarCenarioPadrao(empresa);

            bool executando = true;
            while (executando)
            {
                ExibirMenu(empresa);
                Console.Write("Escolha uma opção: ");
                string entrada = Console.ReadLine()?.Trim();
                Console.WriteLine();

                try
                {
                    switch (entrada)
                    {
                        case "0":
                            executando = false;
                            Console.WriteLine("Encerrando o sistema...");
                            break;

                        case "1":
                            CadastrarVeiculoUI(empresa);
                            break;

                        case "2":
                            CadastrarGaragemUI(empresa);
                            break;

                        case "3":
                            empresa.IniciarJornada();
                            Console.WriteLine("Jornada diária iniciada e veículos distribuídos alternadamente entre as garagens.");
                            ExibirResumoGaragens(empresa);
                            break;

                        case "4":
                            List<string> relatorio = empresa.EncerrarJornada();
                            foreach (var linha in relatorio)
                                Console.WriteLine(linha);
                            break;

                        case "5":
                            LiberarViagemUI(empresa);
                            break;

                        case "6":
                            ListarVeiculosGaragemUI(empresa);
                            break;

                        case "7":
                            ConsultarQuantidadeViagensUI(empresa);
                            break;

                        case "8":
                            ListarViagensUI(empresa);
                            break;

                        case "9":
                            ConsultarQuantidadePassageirosUI(empresa);
                            break;

                        default:
                            Console.WriteLine("Opção inválida. Tente novamente.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Erro] {ex.Message}");
                }

                if (executando)
                {
                    Console.WriteLine("\nPressione ENTER para continuar...");
                    Console.ReadLine();
                    Console.Clear();
                }
            }
        }

        private static void InicializarCenarioPadrao(Empresa empresa)
        {
            empresa.CadastrarGaragem(new Garagem("Congonhas", capacidadeMaxima: 20));
            empresa.CadastrarGaragem(new Garagem("Guarulhos", capacidadeMaxima: 20));

            // Frota inicial de 8 vans (numeradas de 1 a 8)
            for (int i = 1; i <= 8; i++)
            {
                empresa.CadastrarVeiculo(new Veiculo(i, capacidadePassageiros: 15));
            }
        }

        private static void ExibirMenu(Empresa empresa)
        {
            string statusJornada = empresa.JornadaIniciada
                ? "INICIADA"
                : (empresa.JornadaEncerrada ? "ENCERRADA / AGUARDANDO INÍCIO" : "NÃO INICIADA");

            Console.WriteLine("======================================================================");
            Console.WriteLine($" {empresa.Nome} | Status da Jornada: {statusJornada}");
            Console.WriteLine("======================================================================");
            Console.WriteLine(" 0. Finalizar");
            Console.WriteLine(" 1. Cadastrar veículo");
            Console.WriteLine(" 2. Cadastrar garagem");
            Console.WriteLine(" 3. Iniciar jornada");
            Console.WriteLine(" 4. Encerrar jornada");
            Console.WriteLine(" 5. Liberar viagem de uma determinada origem para um determinado destino");
            Console.WriteLine(" 6. Listar veículos em determinada garagem (qtd e potencial de transporte)");
            Console.WriteLine(" 7. Informar quantidade de viagens efetuadas (origem -> destino)");
            Console.WriteLine(" 8. Listar viagens efetuadas (origem -> destino)");
            Console.WriteLine(" 9. Informar quantidade de passageiros transportados (origem -> destino)");
            Console.WriteLine("======================================================================");
        }

        private static void CadastrarVeiculoUI(Empresa empresa)
        {
            Console.Write("Número do veículo: ");
            if (!int.TryParse(Console.ReadLine(), out int numero))
                throw new ArgumentException("Número do veículo inválido.");

            Console.Write("Capacidade de passageiros (lotação): ");
            if (!int.TryParse(Console.ReadLine(), out int capacidade))
                throw new ArgumentException("Capacidade inválida.");

            empresa.CadastrarVeiculo(new Veiculo(numero, capacidade));
            Console.WriteLine($"Veículo #{numero} (lotação {capacidade}) cadastrado com sucesso.");
        }

        private static void CadastrarGaragemUI(Empresa empresa)
        {
            Console.Write("Nome da garagem (ex.: Viracopos): ");
            string nome = Console.ReadLine();

            Console.Write("Capacidade máxima de veículos na garagem: ");
            if (!int.TryParse(Console.ReadLine(), out int capacidade))
                throw new ArgumentException("Capacidade máxima inválida.");

            empresa.CadastrarGaragem(new Garagem(nome, capacidade));
            Console.WriteLine($"Garagem '{nome}' cadastrada com sucesso.");
        }

        private static void LiberarViagemUI(Empresa empresa)
        {
            Garagem origem = SelecionarGaragem(empresa, "origem");
            Garagem destino = SelecionarGaragem(empresa, "destino");

            Viagem viagem = empresa.LiberaViagem(origem, destino);
            Console.WriteLine("Viagem liberada e concluída com sucesso:");
            Console.WriteLine($"  {viagem}");
        }

        private static void ListarVeiculosGaragemUI(Empresa empresa)
        {
            Garagem garagem = SelecionarGaragem(empresa, "consulta");
            List<Veiculo> veiculos = empresa.ListarVeiculos(garagem);
            int potencial = garagem.PotencialTransporte();

            Console.WriteLine($"Garagem: {garagem.Nome}");
            Console.WriteLine($"Quantidade de veículos estacionados: {veiculos.Count}");
            Console.WriteLine($"Potencial de transporte: {potencial} passageiro(s)");

            if (veiculos.Count == 0)
            {
                Console.WriteLine("  (Nenhum veículo na garagem no momento)");
                return;
            }

            Console.WriteLine("  Ordem de saída (Topo da pilha -> Base):");
            for (int i = 0; i < veiculos.Count; i++)
            {
                string marcador = i == 0 ? " [PRÓXIMO A SAIR]" : "";
                Console.WriteLine($"  {i + 1}. {veiculos[i]}{marcador}");
            }
        }

        private static void ConsultarQuantidadeViagensUI(Empresa empresa)
        {
            Garagem origem = SelecionarGaragem(empresa, "origem");
            Garagem destino = SelecionarGaragem(empresa, "destino");

            int qtd = empresa.ConsultarViagens(origem, destino).Count;
            Console.WriteLine($"Quantidade de viagens de {origem.Nome} para {destino.Nome}: {qtd}");
        }

        private static void ListarViagensUI(Empresa empresa)
        {
            Garagem origem = SelecionarGaragem(empresa, "origem");
            Garagem destino = SelecionarGaragem(empresa, "destino");

            List<Viagem> viagens = empresa.ConsultarViagens(origem, destino);
            Console.WriteLine($"Viagens de {origem.Nome} para {destino.Nome} (Total: {viagens.Count}):");

            if (viagens.Count == 0)
            {
                Console.WriteLine("  Nenhuma viagem registrada neste trecho.");
                return;
            }

            foreach (var v in viagens)
            {
                Console.WriteLine($"  - {v}");
            }
        }

        private static void ConsultarQuantidadePassageirosUI(Empresa empresa)
        {
            Garagem origem = SelecionarGaragem(empresa, "origem");
            Garagem destino = SelecionarGaragem(empresa, "destino");

            int totalPassageiros = empresa.ConsultarPassageiros(origem, destino);
            Console.WriteLine($"Total de passageiros transportados de {origem.Nome} para {destino.Nome}: {totalPassageiros}");
        }

        private static Garagem SelecionarGaragem(Empresa empresa, string papel)
        {
            if (empresa.Garagens.Count == 0)
                throw new InvalidOperationException("Não há garagens cadastradas.");

            Console.WriteLine($"Garagens disponíveis ({papel}):");
            for (int i = 0; i < empresa.Garagens.Count; i++)
            {
                Garagem g = empresa.Garagens[i];
                Console.WriteLine($"  {i + 1}. {g.Nome} ({g.Veiculos.Size()}/{g.CapacidadeMaxima} veículos)");
            }

            Console.Write($"Digite o número ou nome da garagem de {papel}: ");
            string entrada = Console.ReadLine()?.Trim();

            if (int.TryParse(entrada, out int idx) && idx >= 1 && idx <= empresa.Garagens.Count)
                return empresa.Garagens[idx - 1];

            Garagem porNome = empresa.BuscarGaragemPorNome(entrada);
            if (porNome != null)
                return porNome;

            throw new ArgumentException("Garagem não encontrada.");
        }

        private static void ExibirResumoGaragens(Empresa empresa)
        {
            foreach (var g in empresa.Garagens)
            {
                var vans = string.Join(", ", g.ListarVeiculos().Select(v => $"#{v.Numero}"));
                Console.WriteLine($"  - {g.Nome}: {g.Veiculos.Size()} veículo(s) -> [{vans}] (Topo -> Base)");
            }
        }
    }
}