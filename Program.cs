using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

// Classe para representar um equipamento audiovisual
class Equipamento
{
    public string Nome { get; set; }
    public string Tipo { get; set; }
    public bool Disponivel { get; set; }

    // Construtor da classe Equipamento
    public Equipamento(string nome, string tipo)
    {
        Nome = nome;
        Tipo = tipo;
        Disponivel = true; // Define o equipamento como disponível por padrão
    }
}

// Classe para representar uma reserva de equipamento
class Reserva
{
    public string Professor { get; set; }
    public Equipamento Equipamento { get; set; }
    public DateTime DataReserva { get; set; }

    // Construtor da classe Reserva
    public Reserva(string professor, Equipamento equipamento, DateTime dataReserva)
    {
        Professor = professor;
        Equipamento = equipamento;
        DataReserva = dataReserva;
    }

    // Método para formatar a reserva em uma linha CSV
    public string ToCSV()
    {
        return $"{Professor},{Equipamento.Nome},{DataReserva}";
    }

    // Método estático para criar uma reserva a partir de uma linha CSV
    public static Reserva FromCSV(string csvLine, List<Equipamento> equipamentos)
    {
        string[] values = csvLine.Split(',');
        string professor = values[0];
        string equipamentoNome = values[1];
        DateTime dataReserva = DateTime.Parse(values[2]);

        Equipamento equipamento = equipamentos.FirstOrDefault(e => e.Nome == equipamentoNome);

        return new Reserva(professor, equipamento, dataReserva);
    }
}

// Classe principal do programa
class Program
{
    static List<Equipamento> equipamentos = new List<Equipamento>(); // Lista de equipamentos disponíveis
    static List<Reserva> reservas = new List<Reserva>(); // Lista de reservas realizadas
    static string equipamentosFile = "equipamentos.csv"; // Arquivo para armazenar os equipamentos
    static string reservasFile = "reservas.csv"; // Arquivo para armazenar as reservas

    // Método principal
    static void Main(string[] args)
    {
        // Inicialização da lista de equipamentos
        equipamentos.AddRange(new List<Equipamento>
    {
        new Equipamento("Microfone", "Som"),
        new Equipamento("Amplificador", "Som"),
        new Equipamento("Mixer de Áudio", "Som"),
        new Equipamento("Caixa de Som Portátil", "Som"),
        new Equipamento("Fone de Ouvido", "Som")
    });
        CarregarReservas(); // Carrega as reservas do arquivo

        // Loop principal do programa
        while (true)
        {
            // Menu de opções
            Console.WriteLine("Escolha uma opção:");
            Console.WriteLine("1. Reservar equipamento");
            Console.WriteLine("2. Cancelar reserva");
            Console.WriteLine("3. Listar equipamentos disponíveis");
            Console.WriteLine("4. Visualizar histórico de reservas");
            Console.WriteLine("5. Sair");

            string opcao = Console.ReadLine(); // Lê a opção escolhida pelo usuário

            // Executa a ação correspondente à opção escolhida
            switch (opcao)
            {
                case "1":
                    RealizarReserva();
                    break;
                case "2":
                    CancelarReserva();
                    break;
                case "3":
                    ListarEquipamentosDisponiveis();
                    break;
                case "4":
                    VisualizarHistoricoReservas();
                    break;
                case "5":
                    SalvarEquipamentos(); // Salva os equipamentos no arquivo
                    SalvarReservas(); // Salva as reservas no arquivo
                    return; // Sai do programa
                default:
                    Console.WriteLine("Opção inválida. Tente novamente.");
                    break;
            }
        }
    }

    // Método para realizar uma reserva
    static void RealizarReserva()
    {
        Console.WriteLine("Digite o nome do professor:");
        string nomeProfessor = Console.ReadLine();

        Console.WriteLine("Escolha o equipamento:");
        for (int i = 0; i < equipamentos.Count; i++)
        {
            if (equipamentos[i].Disponivel)
            {
                Console.WriteLine($"{i + 1}. {equipamentos[i].Nome}");
            }
        }

        int escolhaEquipamento = int.Parse(Console.ReadLine()) - 1;

        // Verifica se a escolha do equipamento é válida
        if (escolhaEquipamento < 0 || escolhaEquipamento >= equipamentos.Count || !equipamentos[escolhaEquipamento].Disponivel)
        {
            Console.WriteLine("Opção inválida ou equipamento indisponível.");
            return;
        }

        Equipamento equipamentoSelecionado = equipamentos[escolhaEquipamento];
        equipamentoSelecionado.Disponivel = false; // Define o equipamento como indisponível

        reservas.Add(new Reserva(nomeProfessor, equipamentoSelecionado, DateTime.Now)); // Adiciona a reserva à lista de reservas

        Console.WriteLine($"Reserva realizada com sucesso para o equipamento {equipamentoSelecionado.Nome}.");
    }

    // Método para cancelar uma reserva
    static void CancelarReserva()
    {
        Console.WriteLine("Digite o nome do professor:");
        string nomeProfessor = Console.ReadLine();

        var reserva = reservas.FirstOrDefault(r => r.Professor == nomeProfessor);

        if (reserva != null)
        {
            reserva.Equipamento.Disponivel = true; // Define o equipamento como disponível novamente
            reservas.Remove(reserva); // Remove a reserva da lista
            Console.WriteLine("Reserva cancelada com sucesso.");
        }
        else
        {
            Console.WriteLine("Nenhuma reserva encontrada para este professor.");
        }
    }

    // Método para listar os equipamentos disponíveis
    static void ListarEquipamentosDisponiveis()
    {
        Console.WriteLine("Equipamentos disponíveis para reserva:");
        for (int i = 0; i < equipamentos.Count; i++)
        {
            if (equipamentos[i].Disponivel)
            {
                Console.WriteLine($"{i + 1}. {equipamentos[i].Nome}");
            }
        }
    }

    // Método para visualizar o histórico de reservas
    static void VisualizarHistoricoReservas()
    {
        Console.WriteLine("Histórico de reservas:");
        foreach (var reserva in reservas)
        {
            Console.WriteLine($"Professor: {reserva.Professor}, Equipamento: {reserva.Equipamento.Nome}, Data: {reserva.DataReserva}");
        }
    }

    // Método para carregar os equipamentos do arquivo
    static void CarregarEquipamentos()
    {
        if (File.Exists(equipamentosFile))
        {
            using (StreamReader sr = new StreamReader(equipamentosFile))
            {
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    string[] values = line.Split(',');
                    equipamentos.Add(new Equipamento(values[0], values[1])); // Adiciona o equipamento à lista
                }
            }
        }
    }

    // Método para carregar as reservas do arquivo
    static void CarregarReservas()
    {
        if (File.Exists(reservasFile))
        {
            using (StreamReader sr = new StreamReader(reservasFile))
            {
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    reservas.Add(Reserva.FromCSV(line, equipamentos)); // Adiciona a reserva à lista
                }
            }
        }
    }

    // Método para salvar os equipamentos no arquivo
    static void SalvarEquipamentos()
    {
        using (StreamWriter sw = new StreamWriter(equipamentosFile))
        {
            foreach (var equipamento in equipamentos)
            {
                sw.WriteLine($"{equipamento.Nome},{equipamento.Tipo}"); // Escreve os dados do equipamento no arquivo
            }
        }
    }

    // Método para salvar as reservas no arquivo
    static void SalvarReservas()
    {
        using (StreamWriter sw = new StreamWriter(reservasFile))
        {
            foreach (var reserva in reservas)
            {
                sw.WriteLine(reserva.ToCSV()); // Escreve os dados da reserva no arquivo
            }
        }
    }
}
