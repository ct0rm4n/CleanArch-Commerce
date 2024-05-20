namespace Core.Dto
{
    public class CnpjDto
    {
        public string cnpj { get; set; }
        public int identificador_matriz_filial { get; set; }
        public string descricao_matriz_filial { get; set; }
        public string razao_social { get; set; }
        public string nome_fantasia { get; set; }
        public int situacao_cadastral { get; set; }
        public string descricao_situacao_cadastral { get; set; }
        public string data_situacao_cadastral { get; set; }
        public int motivo_situacao_cadastral { get; set; }
        public object nome_cidade_exterior { get; set; }
        public int codigo_natureza_juridica { get; set; }
        public string data_inicio_atividade { get; set; }
        public int cnae_fiscal { get; set; }
        public string cnae_fiscal_descricao { get; set; }
        public string descricao_tipo_logradouro { get; set; }
        public string logradouro { get; set; }
        public string numero { get; set; }
        public string complemento { get; set; }
        public string bairro { get; set; }
        public int cep { get; set; }
        public string uf { get; set; }
        public int codigo_municipio { get; set; }
        public string municipio { get; set; }
        public string ddd_telefone_1 { get; set; }
        public object ddd_telefone_2 { get; set; }
        public object ddd_fax { get; set; }
        public int qualificacao_do_responsavel { get; set; }
        public int capital_social { get; set; }
        public int porte { get; set; }
        public string descricao_porte { get; set; }
        public bool opcao_pelo_simples { get; set; }
        public object data_opcao_pelo_simples { get; set; }
        public object data_exclusao_do_simples { get; set; }
        public bool opcao_pelo_mei { get; set; }
        public object situacao_especial { get; set; }
        public object data_situacao_especial { get; set; }
        public List<CnaesSecundario> cnaes_secundarios { get; set; }
        public List<Qsa> qsa { get; set; }
    }

    public class CnaesSecundario
    {
        public int codigo { get; set; }
        public string descricao { get; set; }
    }

    public class Qsa
    {
        public int identificador_de_socio { get; set; }
        public string nome_socio { get; set; }
        public string cnpj_cpf_do_socio { get; set; }
        public int codigo_qualificacao_socio { get; set; }
        public int percentual_capital_social { get; set; }
        public string data_entrada_sociedade { get; set; }
        public object cpf_representante_legal { get; set; }
        public object nome_representante_legal { get; set; }
        public object codigo_qualificacao_representante_legal { get; set; }
    }


}