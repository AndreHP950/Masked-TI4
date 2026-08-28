# Masked-TI4
Repositório TI4 André Henriques Parreiras, Daniel Felipe Coelho de Freitas, João Pedro Xavier Crispi, Lucas Bozzi de Melo, Lucas de Lima e Silva, Matheus Alvarenga de Araujo, Pedro Cerceau Kreuzer, Gabriel Ramos Torres 

# PASSADO / FUTURO
**Plataforma 3D · Unity · Viagem no Tempo**
---
## Visão Geral
| Gênero | Engine | Tom | Escopo |
|---|---|---|---|
| Plataforma 3D | Unity | Silencioso · Sombrio · Contemplativo | 3+ níveis, produção modular |
**Gancho narrativo:** Um homem preso em loop temporal, no mesmo lugar, através de três eras. O cenário é reaproveitado entre os períodos — menos geometria nova, mais reaproveitamento de level design.
---
## Inegociáveis
- Plataforma 3D na Unity, câmera de movimentação livre
- Sem ações violentas pelo jogador (cenário pode ter explosões, cordas por arco e flecha — o jogador nunca ataca)
- Tema principal: **Passado / Futuro**
---
## Direção de Arte
**Estilo:** Medium-poly + foco em iluminação/VFX, pipeline URP da Unity.
**Referências visuais:**
| Referência | O que foi absorvido |
|---|---|
| **INSIDE** | Iluminação dramática, enquadramento próximo e atmosférico, câmera como ferramenta de clima |
| **SUPERHOT** | Formas limpas, silhueta clara acima de detalhe, geometria propositalmente simples |
| **ULTRAKILL** | Estilo estilizado não-realista que ainda lê como tenso sem exigir modelagem de alta fidelidade |
**Pipeline técnico:**
- URP Post-Processing Volume
- Light Bakes
- Bloom / Vignette
> A iluminação é o item crítico — se ela for fraca, a arte inteira perde força. Trocar realismo pleno por estilização + luz é a decisão que mais reduz risco de produção.
---
## Ambientação — Um Espaço, Três Eras
O mesmo cenário base é reaproveitado nos três períodos. Muda a luz, a decadência, elementos da geometria e os obstáculos; o layout geral permanece o mesmo.
| Era | Estética | Uso no jogo |
|---|---|---|
| **Presente** | Subúrbio contemporâneo reconhecível | Prólogo e epílogo — ponto de partida emocional |
| **Futuro** | TRON: prédios futuristas, neon, linhas marcantes | A casa vira complexo/fábrica — geometria base compartilhada |
| **Superfuturo** | Pós-apocalíptico (ref. Horizon Zero Dawn) | Ruínas do próprio Futuro, natureza/decadência — geometria base compartilhada |
---
## Narrativa
**Estrutura:** Perseguição + loop temporal.
- A cada trecho concluído, o jogador é perseguido pelo homem mascarado no intervalo até a próxima missão.
- **Twist:** o homem mascarado é o próprio protagonista. Em algum ponto, o jogador perde a máquina de portais e é obrigado a colocar máscara de gás — e então vê sua versão do passado entrando na instalação com a máquina. Ele precisa pegá-la. O perseguidor vira perseguido.
- **Tema:** aprender a seguir em frente depois de erros do passado — o loop só quebra se o protagonista decide parar de fugir, se perseguir e se enfrentar.
**Referências narrativas:**
| Referência | O que foi absorvido |
|---|---|
| LIMBO / INSIDE | Narrativa 100% ambiental, sem diálogo, tom e silêncio |
| TENET | Estrutura não-linear, protagonista como peça do quebra-cabeça temporal |
| DARK (Netflix) | Loop temporal pessoal, mesmo lugar em eras diferentes, repetição dos próprios erros |
**Mecânica de apoio — Fantasma:** A movimentação inicial do jogador é gravada e reaparece depois. O jogador literalmente vê e reencontra o seu próprio passado em movimento.
---
## Câmera
Duas opções em avaliação, pendente resultado do protótipo de movimento:
| | **Opção A — 1ª pessoa** | **Opção B — 3ª pessoa fixa** |
|---|---|---|
| Inspiração | Beton Brutal | INSIDE / Mario |
| Implementação | Cinemachine POV + Animation Rigging (mãos) | Cinemachine FreeLook + Collider |
| Prós | Carga de animação menor; câmera não sofre clipping | Melhor leitura espacial; baixo risco de implementação |
| Contras | Menos legível para julgar distância em wall run / ledge climb | Corpo inteiro precisa de animação completa; clipping em alta velocidade |
---
## Mecânicas-Chave
### Wall Run + Ledge Climb → Parkour
- Detecção via raycast lateral (simples e barata em performance)
- Ledge climb exige transição de animação suave — maior custo de arte da mecânica
- **Atenção:** Se câmera 3ª pessoa for escolhida, todas as transições precisam de animação de corpo inteiro
### Portais (controlados pelo jogador)
- Portais no **tempo**, não no espaço — abertos pelo próprio jogador, como na narrativa
- Implementação via Screen Fade (UI) + Scene/Timeline Swap
- Render Texture em tempo real descartada (instável em performance para o tamanho da equipe)
### Mecânica Fantasma (Replay)
- Transform buffer gravado via `List<T>` + ScriptableObject para dados de replay
- Referência de implementação: Super Meat Boy, jogos de corrida (Gran Turismo ghost)
- **Candidata a corte** se o prazo apertar — risco de dessincronização com física de parkour
---
## Controlador de Movimento
| Opção | Por quê não |
|---|---|
| `CharacterController` nativo | Cinemático, não responde a forças, rampas ou transferência de momentum |
| **Rigidbody customizado (KCC)** ✓ | Baseado em física real; padrão de facto da indústria indie para movimento baseado em momentum |
---
## Referências de Gameplay
| Aspecto | Referência | O que foi absorvido |
|---|---|---|
| Movimento | Beton Brutal | Parkour arcade puro, travessia como desafio central |
| Movimento | Ghostrunner | Mãos visíveis, sensação de peso via câmera |
| Movimento | Mirror's Edge | Momentum e fluidez entre movimentos |
| Câmera | INSIDE | Enquadramento próximo e atmosférico |
| Progressão | Sonic 3D | Plataforma baseada em velocidade/momentum |
| Traversal | Portal | Ferramenta de progressão e resolução de problemas |
---
## Riscos Técnicos
| Risco | Por quê | Mitigação |
|---|---|---|
| Feel do movimento | Não é código isolado — é iteração | Prototipar o controlador antes de qualquer outra feature |
| IA de perseguição | Funcionar bem em múltiplos níveis e períodos é não-trivial | Começar com trigger + rota fixa; complexificar só se sobrar tempo |
| Replay-fantasma | Gravar/reproduzir movimento sincronizado é propenso a bugs sutis | Tratar como candidata a corte se o prazo apertar |
| Escopo modular | 'Modular' ainda não está definido na prática | Definir quais elementos são reutilizados antes de comprometer o cronograma |
---
## Pontos em Aberto
- [ ] **Câmera definitiva** — aguardando resultado do protótipo de movimento (1ª vs 3ª pessoa)
- [ ] **Enredo detalhado** — progressão exata da perseguição e como o loop se quebra
- [ ] **Capacidade de arte** — se câmera 3ª pessoa, a equipe consegue sustentar animação de corpo inteiro?
- [ ] **Definição de 'modular'** — quais elementos (geometria, mecânicas, encontros) serão reutilizados entre os níveis
---
## Documentação de Level Design
| Arquivo | Descrição |
|---|---|
| `PROPOSTA DE JOGO - TI4.pdf` | Pitch completo do projeto — visão geral, arte, narrativa, mecânicas e riscos |
