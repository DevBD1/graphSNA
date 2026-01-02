# 1. Proje Hakkinda Temel Bilgiler
## 1.1 graphSNA Nedir?
**graphSNA**, Kocaeli Üniversitesi Bilişim Sistemleri Mühendisliği Bölümü, **Yazılım Geliştirme Laboratuvarı-I** dersi kapsamında geliştirilmiş bir masaüstü uygulamasıdır. Bu proje, karmaşık sosyal ağ yapılarını matematiksel bir **Graf (Graph)** modeli olarak ele alır; kullanıcıları ve aralarındaki etkileşimleri görselleştirerek üzerinde gelişmiş analiz algoritmalarının çalıştırılmasını sağlar. Kısaca bu bir "Sosyal Ağ Analizi ve Görselleştirme Aracı" (Social Network Analysis and Visualization Tool) olarak adlandırılabilir.

## 1.2 Ekip Uyeleri
- Mehmet Burak Dorman - Öğrenci No: 251307120 - Geliştirici
- Hüseyin Erekmen - Öğrenci No: 251307099 - Geliştirici

## 1.3 Tarihler
- Proje Başlangıç Tarihi: 25 Kasım 2025
- Proje Tamamlanış Tarihi: ?
---
# 2. Tanım ve Amaç
## 2.1 Problemin Tanımı
Günümüz dijital dünyasında bireyler ve topluluklar arasındaki ilişkiler (sosyal ağlar) giderek karmaşıklaşmaktadır. Milyonlarca bağlantının olduğu bir yapıda;
* İki kişi arasındaki en kısa iletişim yolunu bulmak,
* Ağdaki en etkili kişiyi tespit etmek,
* Birbirinden kopuk toplulukları ayrıştırmak,
gibi problemler çıplak gözle veya basit listelerle çözülemez hale gelmiştir. Bu verilerin anlamlandırılabilmesi için düğüm (node) ve kenar (edge) tabanlı görselleştirme tekniklerine ve algoritmik analizlere ihtiyaç duyulmaktadır.

## 2.2 Projenin Amacı
Bir görsel programlama dili olan C# kullanarak ve NYP (OOP) prensiplerine bağlı kalarak tasarlanan bu program ile amaç, kullanıcıların dinamik olarak yönetilebildiği ve analiz edilebildiği interaktif bir sistem tasarlamaktır.

Uygulama, kullanıcı özelliklerine (aktiflik, etkileşim sayısı, bağlantı sayısı) dayalı **dinamik ağırlık hesaplaması** yaparak standart graflardan farklılaşır. Aşağıdaki temel yetenekleri sunmayı hedefler:
* **Görselleştirme:** Kullanıcıların ve bağlantıların bir tuval (canvas) üzerinde çizilmesi.
* **Yol Analizi:** Dijkstra ve A* algoritmaları ile en maliyetsiz rotaların bulunması.
* **Topluluk Analizi:** Merkezilik (Centrality) hesapları ve BFS/DFS ile ağın taranması.
* **Gruplama:** Welsh-Powell algoritması ile ayrık toplulukların renklendirilerek görselleştirilmesi.
* **Veri Depolama:** Ağ yapısının JSON/CSV formatında saklanması ve taşınabilmesi.
---
## 3. Algoritmalar
### 3.1.1 Breadth-First Search (BFS)
Çalışma Mantığı: FS, bir graf üzerinde seviye seviye ilerleyen bir arama algoritmasıdır. Başlangıç düğümünden başlayarak önce tüm komşuları ziyaret eder, ardından komşuların komşularına geçer. FIFO (First In First Out) prensibiyle çalışan Queue (Kuyruk) veri yapısı kullanır.
Adımlar:
1.	Başlangıç düğümünü kuyruğa ekle ve ziyaret edildi olarak işaretle
2.	Kuyruktan bir düğüm çıkar, sonuç listesine ekle
3.	Bu düğümün ziyaret edilmemiş tüm komşularını kuyruğa ekle
4.	Kuyruk boşalana kadar 2-3 adımlarını tekrarla

```mermaid
flowchart TD
    A["Start"] --> B["Initialize Queue, Visited Set, Result List"]
    B --> C["Enqueue startNode"]
    C --> D["Mark startNode as visited"]
    D --> E{"Queue empty?"}
    E -->|Yes| F["Return Result List"]
    E -->|No| G["Dequeue current node"]
    G --> H["Add current to Result"]
    H --> I["Get all neighbors of current"]
    I --> J{"More unvisited neighbors?"}
    J -->|Yes| K["Mark neighbor as visited"]
    K --> L["Enqueue neighbor"]
    L --> J
    J -->|No| E
```
### 3.1.2 Karmaşıklık Analizi
| Metrik             | Değer    | Açıklama                                                                   |
|--------------------|----------|----------------------------------------------------------------------------|
| Zaman Karmaşıklığı | O(V + E) | Her düğüm (V) bir kez ziyaret edilir, her kenar (E) bir kez kontrol edilir |
| Alan Karmaşıklığı  | O(V)     | Queue ve Visited set en fazla V eleman tutar                               |

### 3.1.3 Literatür İncelemesi
- İlk Tanım: Edward F. Moore (1959) - Labirent çözümü için geliştirildi
- Kullanım Alanları:
- - Sosyal ağlarda "arkadaş önerisi" (2 derece uzaklıktaki kişiler)
- - GPS navigasyonunda en az aktarmalı rota bulma
- - Web crawler'larda sayfa indeksleme
- - Ağ yayılımı (broadcast) protokolleri
- Kaynak: Cormen, T. H., et al. "Introduction to Algorithms" (2009), Chapter 22.2

---
### 3.2.1 DFS (Depth-First Search) - Derinlik Öncelikli Arama
Çalışma Mantığı: DFS, bir dalı sonuna kadar takip eden ve ardından geri dönerek diğer dalları keşfeden bir arama algoritmasıdır. LIFO (Last In First Out) prensibiyle çalışan Stack (Yığın) veri yapısı kullanır. Bir yolda gidebildiği kadar derine iner, çıkmaza girince geri döner (backtracking).
Adımlar:
1.	Başlangıç düğümünü stack'e ekle
2.	Stack'ten bir düğüm çıkar (pop)
3.	Ziyaret edilmediyse: ziyaret edildi olarak işaretle, sonuç listesine ekle
4.	Tüm komşularını stack'e ekle
5.	Stack boşalana kadar 2-4 adımlarını tekrarla

```mermaid
flowchart TD
    A["Start"] --> B["Initialize Stack, Visited Set, Result List"]
    B --> C["Push startNode to Stack"]
    C --> D{"Stack empty?"}
    D -->|Yes| E["Return Result List"]
    D -->|No| F["Pop current node from Stack"]
    F --> G{"current visited?"}
    G -->|Yes| D
    G -->|No| H["Mark current as visited"]
    H --> I["Add current to Result"]
    I --> J["Get all neighbors of current"]
    J --> K["Reverse neighbors order"]
    K --> L{"More neighbors?"}
    L -->|Yes| M{"Neighbor visited?"}
    M -->|Yes| L
    M -->|No| N["Push neighbor to Stack"]
    N --> L
    L -->|No| D
```
### 3.2.2 Karmaşıklık Analizi
| Metrik             | Değer    | Açıklama                                                                   |
|--------------------|----------|----------------------------------------------------------------------------|
| Zaman Karmaşıklığı | O(V + E) | Her düğüm ve kenar bir kez işlenir |
| Alan Karmaşıklığı  | O(V)     | Stack derinliği en kötü durumda V olabilir                               |

### 3.2.3 Literatür İncelemesi
- İlk Tanım: Charles Pierre Trémaux (19. yüzyıl) - Labirent çözümü
- Kullanım Alanları:
- - Topolojik sıralama (görev bağımlılıkları)
- - Döngü tespiti (cycle detection)
- - Bağlı bileşen analizi
- - Puzzle çözümü (Sudoku, satranç)
- - Derleyicilerde syntax tree traversal
- Kaynak: Tarjan, R. E. "Depth-First Search and Linear Graph Algorithms" (1972), SIAM Journal on Computing

---
### 3.3.1 Dijkstra Algoritması
Çalışma Mantığı: Dijkstra, ağırlıklı graflarda tek bir kaynaktan diğer tüm düğümlere en kısa yolu bulan bir algoritmadır. Greedy (Açgözlü) yaklaşım kullanır: her adımda en düşük maliyetli düğümü seçer. Priority Queue (Öncelik Kuyruğu) veri yapısı ile optimize edilir.
Adımlar:
1.	Başlangıç düğümünün mesafesini 0, diğerlerini ∞ olarak ayarla
2.	Priority Queue'dan en küçük mesafeli düğümü çıkar
3.	Bu düğümün her komşusu için: yeni mesafe daha kısaysa güncelle
4.	Hedef düğüme ulaşana veya kuyruk boşalana kadar tekrarla
5.	Yolu geriye doğru takip ederek reconstruct et

```mermaid
flowchart TD
    A["Start"] --> B["Set distance[start] = 0"]
    B --> C["Set distance[others] = ∞"]
    C --> D["Enqueue start with priority 0"]
    D --> E{"PriorityQueue empty?"}
    E -->|Yes| F["Return: No path found"]
    E -->|No| G["Dequeue node with minimum distance"]
    G --> H{"current == goal?"}
    H -->|Yes| I["Reconstruct path using previous map"]
    I --> J["Return path and total cost"]
    H -->|No| K["For each neighbor of current"]
    K --> L["Calculate newDist = distance[current] + edgeWeight"]
    L --> M{"newDist < distance[neighbor]?"}
    M -->|Yes| N["Update distance[neighbor] = newDist"]
    N --> O["Set previous[neighbor] = current"]
    O --> P["Enqueue neighbor with newDist"]
    P --> Q{"More neighbors?"}
    M -->|No| Q
    Q -->|Yes| K
    Q -->|No| E
```
### 3.3.2 Karmaşıklık Analizi
| Metrik             | Değer    | Açıklama                                                                   |
|--------------------|----------|----------------------------------------------------------------------------|
| Zaman Karmaşıklığı | O((V + E) log V) | Priority Queue ile; her düğüm/kenar için log V'lik işlem |
| Alan Karmaşıklığı  | O(V)     | Distance ve Previous dictionary'leri                               |

### 3.3.3 Literatür İncelemesi
- İlk Tanım: Edsger W. Dijkstra (1956) - "A note on two problems in connexion with graphs"
- Kısıtlama: Negatif ağırlıklı kenarlarda çalışmaz (Bellman-Ford gerekir)
- Kullanım Alanları:
- - Google Maps, Yandex Navigasyon rota hesaplama
- - Network routing protokolleri (OSPF, IS-IS)
- - Telekomünikasyonda çağrı yönlendirme
- - Oyunlarda NPC pathfinding
- Kaynak: Dijkstra, E. W. "A note on two problems in connexion with graphs" (1959), Numerische Mathematik

---
### 3.4.1 A* (A-Star) Algoritması
Çalışma Mantığı: A*, Dijkstra'nın geliştirilmiş halidir. Heuristic (Sezgisel) fonksiyon kullanarak hedefe yönelik arama yapar. 
Değerlendirme fonksiyonu: f(n) = g(n) + h(n)
- g(n): Başlangıçtan n'e olan gerçek maliyet
- h(n): n'den hedefe tahmini maliyet (heuristic)
Projede Euclidean Distance (Öklid Mesafesi) heuristic olarak kullanılmıştır.
Adımlar:
1.	Dijkstra ile aynı başlangıç
2.	Priority Queue'ya eklerken: f = g + h değerini kullan
3.	Hedefe yakın düğümler öncelikli işlenir
4.	Optimal yol garantisi (admissible heuristic ile)

```mermaid
flowchart TD
    A["Start"] --> B["Set g[start] = 0, g[others] = ∞"]
    B --> C["Calculate h[start] = Euclidean distance to goal"]
    C --> D["Enqueue start with priority f = g + h"]
    D --> E{"PriorityQueue empty?"}
    E -->|Yes| F["Return: No path found"]
    E -->|No| G["Dequeue node with minimum f value"]
    G --> H{"current == goal?"}
    H -->|Yes| I["Reconstruct path"]
    I --> J["Return path and g[goal] as cost"]
    H -->|No| K["For each neighbor of current"]
    K --> L["Calculate newG = g[current] + edgeWeight"]
    L --> M{"newG < g[neighbor]?"}
    M -->|Yes| N["Update g[neighbor] = newG"]
    N --> O["Set previous[neighbor] = current"]
    O --> P["Calculate f = newG + h[neighbor]"]
    P --> Q["Enqueue neighbor with priority f"]
    Q --> R{"More neighbors?"}
    M -->|No| R
    R -->|Yes| K
    R -->|No| E
```
### 3.4.2 Karmaşıklık Analizi
| Metrik             | Değer    | Açıklama                                                                   |
|--------------------|----------|----------------------------------------------------------------------------|
| Zaman Karmaşıklığı | O((V + E) log V) | En kötü durumda Dijkstra ile aynı |
| Alan Karmaşıklığı  | O(V)     | Distance, Previous ve Priority Queue Pratikte Dijkstra'dan hızlı Heuristic sayesinde daha az düğüm ziyaret edilir |

### 3.4.3 Literatür İncelemesi
- İlk Tanım: Peter Hart, Nils Nilsson, Bertram Raphael (1968) - Stanford Research Institute
- Makale: "A Formal Basis for the Heuristic Determination of Minimum Cost Paths"
- Heuristic Türleri:
- - Euclidean Distance: √((x₁-x₂)² + (y₁-y₂)²)
- - Manhattan Distance: |x₁-x₂| + |y₁-y₂|
- - Chebyshev Distance: max(|x₁-x₂|, |y₁-y₂|)
- Kullanım Alanları:
- - Video oyunlarında pathfinding (Unity, Unreal Engine)
- - Robotik navigasyon
- - Harita uygulamalarında gerçek zamanlı rota
- Kaynak: Hart, P. E., et al. "A Formal Basis for the Heuristic Determination of Minimum Cost Paths" (1968), IEEE Transactions on Systems Science and Cybernetics

---
### 3.5.1 Connected Components
Çalışma Mantığı: Bu algoritma, izole edilmiş alt grafikleri tanımlar. Her düğümü tek tek inceler; eğer bir düğüm ziyaret edilmemişse, oradan ulaşılabilir tüm düğümleri bulmak için bir geçiş (BFS/DFS) başlatır ve bunları tek bir "bileşen" halinde gruplandırır.

```mermaid
flowchart TD
    A([Start: Input Graph]) --> B[Init Visited Set & Components List]
    B --> C[Iterate All Nodes]
    C --> D{Is Node Visited?}
    D -- Yes --> C
    D -- No --> E[Start New Component List]
    E --> F[Run BFS/DFS from Node]
    F --> G[Mark All Reachable as Visited]
    G --> H[Add to Current Component List]
    H --> I[Add Component to Master List]
    I --> C
    C -- All Processed --> J([End: Return List of Lists])
```
### 3.5.2 Karmaşıklık Analizi
| Metrik             | Değer    | Açıklama                                                                   |
|--------------------|----------|----------------------------------------------------------------------------|
| Zaman Karmaşıklığı | O(V + E) | Çünkü her düğümü ve kenarı tam olarak bir kez ziyaret eder. |

### 3.5.3 Literatür İncelemesi
- Ağ analizinin temel unsurlarından biri, ağın dayanıklılığını belirlemektir. Bir ağın 1 bileşeni varsa, tamamen bağlıdır. Birden fazla bileşeni varsa, parçalanmıştır.

---
### 3.6.1 Graph Coloring (Welsh-Powell)
Çalışma Mantığı: Bir grafı, bitişik iki düğümün aynı renge sahip olmaması koşuluyla renklendirmek için kullanılan bir algoritma. Düğümleri bağlantı sayısına (dereceye) göre azalan sırada sıralar. İlk rengi en yüksek dereceli düğüme ve ardından bitişik olmayan tüm düğümlere atar, sonra bir sonraki renge geçer.

```mermaid
flowchart TD
    A([Start: Input Graph]) --> B[Calculate Degrees of All Nodes]
    B --> C[Sort Nodes Descending by Degree]
    C --> D[Init ColorIndex = 0]
    D --> E{Are All Nodes Colored?}
    E -- Yes --> F([End: Return ColorCount])
    E -- No --> G[Select Uncolored Node with Max Degree]
    G --> H["Assign Color[ColorIndex]"]
    H --> I[Iterate Other Uncolored Nodes]
    I --> J{Is Node Adjacent to ANY Node of Current Color?}
    J -- No --> K["Assign Color[ColorIndex]"]
    J -- Yes --> I
    K --> I
    I -- List Done --> L[Increment ColorIndex]
    L --> E
```
### 3.6.2 Karmaşıklık Analizi
| Metrik             | Değer    | Açıklama                                                                   |
|--------------------|----------|----------------------------------------------------------------------------|
| Zaman Karmaşıklığı | O(V^2 + E) | Sıralama ve komşuluk kontrolü nedeniyle |

### 3.6.3 Literatür İncelemesi
- 1967'de D. J. A. Welsh ve M. B. Powell tarafından önerilmiştir. Bir grafın kromatik sayısı için upper bound sağlar ve zamanlama problemlerinde (örneğin, sınav zamanlaması) ve derleyicilerde register tahsisinde kullanılır.

---
### 3.7.1 Algoritma Karşılaştırma Tablosu
| Algoritma | Zaman        | Alan | Ağırlıklı Graf | Optimal Yol    | Kullanım Senaryosu                  |
|-----------|--------------|------|----------------|----------------|-------------------------------------|
| BFS       | O(V+E)       | O(V) | ❌              | ✅ (ağırlıksız) | Seviye bazlı keşif, arkadaş önerisi |
| DFS       | O(V+E)       | O(V) | ❌              | ❌              | Döngü tespiti, topolojik sıralama   |
| Dijkstra  | O((V+E)logV) | O(V) | ✅              | ✅              | Ağırlıklı en kısa yol               |
| A*        | O((V+E)logV) | O(V) | ✅              | ✅              | Koordinat bilgisi varsa hızlı yol   |

### 3.7.2 Kaynakça 
1.	Cormen, T. H., Leiserson, C. E., Rivest, R. L., & Stein, C. (2009). Introduction to Algorithms (3rd ed.). MIT Press.
2.	Dijkstra, E. W. (1959). A note on two problems in connexion with graphs. Numerische Mathematik, 1(1), 269-271.
3.	Hart, P. E., Nilsson, N. J., & Raphael, B. (1968). A formal basis for the heuristic determination of minimum cost paths. IEEE Transactions on Systems Science and Cybernetics, 4(2), 100-107.
4.	Tarjan, R. E. (1972). Depth-first search and linear graph algorithms. SIAM Journal on Computing, 1(2), 146-160.
5.	Moore, E. F. (1959). The shortest path through a maze. Proceedings of the International Symposium on the Theory of Switching, 285-292.

---
## 4. Sınıflar 

```mermaid
classDiagram
    %% Relationships - Core Architecture
    MainAppForm --> Controller : Uses
    InputNodeForm <-- MainAppForm : Instantiates
    Controller --> Graph : Manages
    Controller --> AlgorithmBase : Executes via Strategy
    
    %% Relationships - Foundation
    Graph "1" *-- "*" Node : Contains
    Graph "1" *-- "*" Edge : Contains
    FileManager ..> Graph : Creates/Saves
    FileManager ..> GraphSerializer : Uses
    
    %% Relationships - Algorithms Inheritance
    BFS --|> AlgorithmBase : Inherits
    DFS --|> AlgorithmBase : Inherits
    
    %% Relationships - Shortest Path Strategy Pattern
    DijkstraAlgorithm --|> AlgorithmBase : Inherits
    AStarAlgorithm --|> AlgorithmBase : Inherits
    
    DijkstraAlgorithm ..|> IShortestPathAlgorithm : Implements
    AStarAlgorithm ..|> IShortestPathAlgorithm : Implements
    
    %% The Manager uses the Interface, not the concrete classes
    ShortestPathManager o-- IShortestPathAlgorithm : Composes
    Controller --> ShortestPathManager : Uses for routing

    %% UI Layer
    class MainAppForm {
        +GraphController controller
        -InitializeComponent()
        -Canvas_Paint()
        -RunTraverse()
        -RunFindShortestPath()
    }
    
    class InputNodeForm {
        +string NodeName
        +float Activity
        +float Interaction
        +InputNodeForm(title)
    }

    class Controller {
        +Graph ActiveGraph
        +LoadGraph(filePath)
        +SaveGraph(filePath)
        +CalculateShortestPath(start, end, type)
        +RecalculateAllWeights()
    }

    %% Foundation Layer
    class Graph {
        +List~Node~ Nodes
        +List~Edge~ Edges
        +AddNode(Node)
        +AddEdge(Node, Node)
    }

    class Node {
        +string Id
        +string Name
        +Point Location
        +Color Color
    }

    class Edge {
        +Node Source
        +Node Target
        +double Weight
    }

    class FileManager {
        +SaveGraph(graph, path)
        +LoadGraph(path)
    }

    class GraphSerializer {
        +ParseCsv(lines)
        +SerializeGraph(graph)
    }
    
    %% ALGORITHMS LAYER
    class AlgorithmBase {
        <<Abstract>>
        +string Name
        +string TimeComplexity
        #ValidateInput()
        #GetNeighbors()
        #GetEdgeWeight()
    }
    
    class BFS {
        +Traverse(graph, start)
    }
    
    class DFS {
        +Traverse(graph, start)
    }
    
    %% SHORTEST PATH MODULE (Strategy Pattern)
    class IShortestPathAlgorithm {
        <<Interface>>
        +FindPath(graph, start, goal)
    }

    class ShortestPathManager {
        -IShortestPathAlgorithm _algorithm
        +SetAlgorithm(algorithm)
        +Calculate(graph, start, target)
    }

    class DijkstraAlgorithm {
        +FindPath(graph, start, goal)
        -ReconstructPath(previous, current)
    }

    class AStarAlgorithm {
        +FindPath(graph, start, goal)
        -Heuristic(nodeA, nodeB)
        -ReconstructPath(previous, current)
    }
```

---
## 5. Uygulamaya Ait Aciklamalara, Ekran görüntüleri ve Test
GraphSNA (Social Network Analysis) uygulaması, karmaşık ağ yapılarını görselleştirmek, analiz etmek ve grafik teorisi algoritmalarını (BFS, DFS, Dijkstra, A*, Renklendirme vb.) bu ağlar üzerinde çalıştırmak amacıyla geliştirilmiş bir Windows Forms uygulamasıdır.

- Uygulamanın temel yetenekleri şunlardır:
- - Veri Yönetimi: .csv formatındaki veri setlerini (ID, İsim, Aktivite, Etkileşim, Komşular formatında) içe aktarabilir ve üzerinde çalışılan grafı dışa aktarabilir. Excel uyumluluğu için UTF-8 BOM desteği sağlanmıştır.
- - Görselleştirme: Düğümler (Nodes) ve Kenarlar (Edges) GDI+ kullanılarak çizilir. "Force-Directed Layout" (Fiziksel Simülasyon) kullanılarak düğümlerin ekran üzerinde en uygun yerleşimi sağlanır.

- Algoritmalar:
- - Gezinme: Breadth-First Search (BFS) ve Depth-First Search (DFS) ile ağ üzerinde düğüm ziyaret sıralaması simüle edilir.
- - En Kısa Yol: İki düğüm arasındaki en maliyet-etkin yol, Dijkstra ve A (A-Star)* algoritmaları kullanılarak hesaplanır. A* algoritması, düğümlerin konumlarına dayalı öklid mesafesini sezgisel (heuristic) olarak kullanır.
- - Renklendirme: Welsh-Powell algoritması ile komşu düğümlerin aynı renge sahip olmayacağı şekilde en az renk sayısı (Kromatik Sayı) hesaplanır.
- - Bağlı Bileşenler: Grafın parçalı olup olmadığı ve izole gruplar analiz edilir.
- - Merkezilik Analizi: Düğümlerin komşu sayısı ve etkileşim puanlarına göre "Top Influencers" (En Etkili Kişiler) listesi oluşturulur.
- - Düğüm Yönetimi: Arayüz üzerinden veya sağ tık menüsü ile yeni düğüm ekleme, silme ve özelliklerini (Activity, Interaction) düzenleme imkanı sunar. Veri girişlerinde 0-100 aralığı ve boş isim kontrolü gibi validasyonlar mevcuttur.

### 5.2. Ekran Görüntüleri 

Ana Ekran ve Yüklü Graf: large_dataset-1.csv dosyasını yükledikten sonra oluşan ağ yapısının görüntüsü.
https://i.ibb.co/5J03dmr/graphsna5.png

En Kısa Yol Analizi: İki düğüm seçip (Örn: Ali ve Ayşe) Dijkstra algoritmasını çalıştırdığınızda çizilen kırmızı yolun görüntüsü.
https://i.ibb.co/fdXMqjY0/graphsna6.png

İstatistik Paneli: Sağ taraftaki "Node Details" paneli ve alttaki "Komşular & Maliyetler" listesinin dolu halinin görüntüsü.
https://i.ibb.co/d0rKqFY3/graphsna7.png
https://i.ibb.co/MywKKLdh/graphsna8.png

Renklendirme Algoritması: "Renklendirme" butonuna bastıktan sonra düğümlerin farklı renklere boyandığı görüntü.
https://i.ibb.co/rKNrK1Bf/graphsna9.png

Validasyon Hatası: Yeni kişi eklerken (InputNodeForm) sayısal değer yerine harf girip "Kaydet" dediğinizde çıkan hata mesajının görüntüsü.
https://i.ibb.co/cX8n5qK4/graphsna10.png

### 5.3. Test Senaryoları ve Sonuçlar
Uygulamanın kararlılığını ve doğruluğunu test etmek için aşağıdaki senaryolar uygulanmıştır.

**Test Senaryosu 1: CSV Veri Yükleme ve Görselleştirme**
Amaç: Harici bir veri kaynağının doğru parse edilmesi ve görselleştirilmesi.
Girdi: large_dataset-1.csv dosyası (İçerik: ID, İsim, 0-100 arası normalize edilmiş Activity ve Interaction değerleri).
Beklenen Sonuç: Dosyadaki tüm düğümlerin ve komşuluk ilişkilerinin hatasız yüklenmesi, Türkçe karakterlerin (ş, ı, ö) bozulmaması.
Gerçekleşen Sonuç: GraphSerializer sınıfı veriyi başarıyla okumuş, "ConnectionCount" değerleri doğru hesaplanmış ve Türkçe karakterler UTF-8 desteği sayesinde düzgün görüntülenmiştir.

**Test Senaryosu 2: Hatalı Veri Girişi (Validasyon)**
Amaç: Kullanıcının düğüm düzenleme formuna geçersiz veri girmesinin engellenmesi.
Adımlar:
Bir düğüme sağ tıklayıp "Düzenle" seçilir.
"Activity" alanına "150" (Sınır dışı) veya "abc" (Metin) girilir.
"Kaydet" butonuna basılır.
Beklenen Sonuç: Uygulamanın işlemi reddetmesi ve uyarı mesajı göstermesi.
Gerçekleşen Sonuç: InputNodeForm sınıfındaki kontrol mekanizması devreye girmiş, "Değerler 0 ile 100 arasında olmalıdır" veya "Geçerli sayısal değer giriniz" uyarısı verilerek kayıt engellenmiştir.

**Test Senaryosu 3: En Kısa Yol (Shortest Path) Doğruluğu**
Amaç: İki düğüm arasındaki maliyetin doğru hesaplanması.
Adımlar:
Başlangıç düğümü seçilir.
Hedef düğüm seçilir.
"Dijkstra" algoritması seçilerek çalıştırılır.
Beklenen Sonuç: Algoritmanın, kenar ağırlıklarını (Activity ve Interaction parametrelerinden türetilen maliyet) dikkate alarak en düşük maliyetli rotayı çizmesi.
Gerçekleşen Sonuç: DijkstraAlgorithm sınıfı, Priority Queue kullanarak en optimum yolu bulmuş ve toplam maliyeti arayüzde göstermiştir. Bulunan yol kırmızı renk ile vurgulanmıştır.

**Test Senaryosu 4: Graf Renklendirme (Welsh-Powell)**
Amaç: Komşu düğümlerin çakışmayacak şekilde renklendirilmesi.
Adımlar: "Renklendirme" butonuna tıklanır.
Beklenen Sonuç: Birbirine doğrudan bağlı (komşu) olan hiçbir düğümün aynı renkte olmaması.
Gerçekleşen Sonuç: Algoritma, düğümleri derecelerine (degree) göre sıralayıp boyamış, işlem sonucunda kullanılan toplam renk sayısı (Kromatik Sayı) raporlanmıştır.

**Test Senaryosu 5: Merkezilik ve İstatistikler**
Amaç: Ağdaki en etkili düğümlerin belirlenmesi.
Adımlar: "Refresh Stats" butonuna basılır ve tablodaki sıralama kontrol edilir.
Beklenen Sonuç: Düğümlerin ConnectionCount * Interaction formülüne göre puanlanıp büyükten küçüğe sıralanması.
Gerçekleşen Sonuç: Controller.GetTopInfluencers metodu doğru çalışmış, "İstatistikler" sekmesindeki DataGridView tablosu en yüksek skora sahip düğümleri (Örn: Cem, Levent gibi yüksek etkileşimli düğümler) en üstte listelemiştir.

---
## 6.1. Sonuç ve Tartışma
### 6.1 Başarılar

Proje kapsamında aşağıdaki hedeflere başarıyla ulaşılmıştır:

#### Algoritma Gerçekleştirmeleri
- **BFS ve DFS:** Her iki gezinme algoritması da başarıyla implemente edildi ve adım adım animasyon desteği eklendi.
- **Dijkstra ve A\*:** Ağırlıklı en kısa yol algoritmaları çalışır durumda; sonuçlar hem görsel (yeşil path) hem de metin olarak sunuluyor.
- **Welsh-Powell:** Graf renklendirme algoritması doğru çalışıyor; komşu düğümler farklı renklere boyanıyor ve kromatik sayı hesaplanıyor.
- **Connected Components:** Bağlı bileşen analizi ile ayrık topluluklar ve izole düğümler tespit edilebiliyor.
- **Degree Centrality:** En etkili 5 kullanıcı tablo halinde gösteriliyor.

#### Yazılım Mimarisi
- **OOP Prensipleri:** `Node`, `Edge`, `Graph`, `AlgorithmBase` gibi ayrı sınıflar oluşturuldu. Interface (`IGraphTraversal`, `IShortestPathAlgorithm`) ve abstract class (`AlgorithmBase`) kullanılarak polimorfizm sağlandı.
- **Kod Tekrarı Önleme:** Ortak metodlar (`GetNeighbors`, `ValidateInput`, `GetEdgeWeight`) base class'a taşınarak DRY prensibi uygulandı.
- **Encapsulation:** `GraphController` sınıfı, UI ile veri katmanı arasında köprü görevi görerek doğrudan veri erişimini engelliyor.

#### Kullanıcı Deneyimi
- **İnteraktif Arayüz:** Zoom/Pan, düğüm sürükleme, sağ tık menüsü gibi kullanıcı dostu özellikler eklendi.
- **Gerçek Zamanlı Geri Bildirim:** Algoritma çalışma süreleri (ms/µs) ve performans metrikleri kullanıcıya gösteriliyor.
- **Animasyonlu Traversal:** BFS/DFS algoritmaları adım adım animasyonla görselleştiriliyor.

#### Veri Yönetimi
- **CSV Import/Export:** Graf verileri CSV formatında kaydedilip yüklenebiliyor.
- **Dinamik Ağırlık Hesaplama:** Proje isterlerindeki formül başarıyla implemente edildi.
- **Komşuluk Matrisi/Listesi:** Hem ekranda gösterme hem de dışa aktarma destekleniyor.

---

### 6.2 Sınırlılıklar

Projenin mevcut versiyonunda aşağıdaki kısıtlamalar bulunmaktadır:

| Sınırlılık | Açıklama |
|------------|----------|
| **Büyük Graf Performansı** | 100+ düğümlü graflarda force-directed layout algoritması yavaşlıyor. 200 düğümde belirgin gecikme yaşanıyor. |
| **Tek Dosya Formatı** | Sadece CSV destekleniyor; JSON import/export özelliği bulunmuyor. |
| **Yönlü Graf Desteği Yok** | Uygulama sadece yönsüz (undirected) grafları destekliyor. Instagram tarzı tek yönlü takip ilişkileri modellenemiyor. |
| **Negatif Ağırlık** | Dijkstra algoritması negatif ağırlıklı kenarlarda çalışmıyor (Bellman-Ford implemente edilmedi). |
| **Çoklu Kenar** | İki düğüm arasında yalnızca bir kenar olabiliyor (multigraph desteği yok). |
| **Geri Alma (Undo)** | Silme veya düzenleme işlemleri geri alınamıyor. |
| **Otomatik Kaydetme** | Uygulama kapanırken otomatik kaydetme uyarısı verilmiyor. |

---

### 6.3 Olası Geliştirmeler

Gelecek versiyonlarda aşağıdaki iyileştirmeler yapılabilir:

#### Algoritma Geliştirmeleri
| Özellik | Açıklama |
|---------|----------|
| **Bellman-Ford Algoritması** | Negatif ağırlıklı graflar için alternatif en kısa yol algoritması |
| **PageRank** | Sosyal ağ analizinde daha gelişmiş merkezilik ölçümü |
| **Community Detection** | Louvain veya Girvan-Newman algoritması ile topluluk tespiti |
| **Minimum Spanning Tree** | Kruskal veya Prim algoritması ile minimum yayılan ağaç |

#### Performans İyileştirmeleri
| Özellik | Açıklama |
|---------|----------|
| **Async/Await Layout** | Force-directed layout hesaplamasının arka planda çalıştırılması |
| **Spatial Indexing** | Quadtree veya R-tree ile büyük graflarda hızlı düğüm araması |
| **GPU Rendering** | OpenGL veya DirectX ile donanım hızlandırmalı çizim |

#### Kullanıcı Deneyimi
| Özellik | Açıklama |
|---------|----------|
| **Undo/Redo** | Command pattern ile geri alma/yineleme desteği |
| **Tema Desteği** | Açık/Koyu mod ve özelleştirilebilir renk şemaları |
| **Graf Şablonları** | Hazır graf yapıları (tam graf, yıldız graf, ağaç vb.) |
| **Çoklu Dil Desteği** | İngilizce ve diğer diller için lokalizasyon |

#### Veri Yönetimi
| Özellik | Açıklama |
|---------|----------|
| **JSON Desteği** | Alternatif dosya formatı olarak JSON import/export |
| **Veritabanı Entegrasyonu** | SQLite veya PostgreSQL ile büyük veri setleri yönetimi |
| **Bulut Senkronizasyonu** | Graf verilerinin bulut üzerinde saklanması |

---

### 6.4 Sonuç

Bu proje, sosyal ağ analizi alanında temel graf algoritmalarının pratik uygulamasını başarıyla göstermektedir. Nesne yönelimli programlama prensipleri, SOLID ilkeleri ve tasarım kalıpları (Strategy Pattern, Template Method) kullanılarak sürdürülebilir bir yazılım mimarisi oluşturulmuştur.

Geliştirme sürecinde karşılaşılan zorluklar (ekran titremesi, karakter kodlama, performans optimizasyonu) sistematik yaklaşımlarla çözülmüş ve bu süreç önemli öğrenme deneyimleri sağlamıştır.

Proje, ders kapsamında istenen tüm işlevsel gereksinimleri karşılamakta olup, yukarıda belirtilen geliştirmelerle daha kapsamlı bir sosyal ağ analiz aracına dönüştürülebilir.
