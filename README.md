# graphSNA Nedir?

**Sosyal Ağ Analizi ve Görselleştirme Aracı**
*(Social Network Analysis and Visualization Tool)*

---

## Proje Hakkında
**graphSNA**, Kocaeli Üniversitesi Bilişim Sistemleri Mühendisliği Bölümü, **Yazılım Geliştirme Laboratuvarı-I** dersi kapsamında geliştirilmiş bir masaüstü uygulamasıdır. Bu proje, karmaşık sosyal ağ yapılarını matematiksel bir **Graf (Graph)** modeli olarak ele alır; kullanıcıları ve aralarındaki etkileşimleri görselleştirerek üzerinde gelişmiş analiz algoritmalarının çalıştırılmasını sağlar.

## Ekip Uyeleri
- Mehmet Burak Dorman - Öğrenci No: 251307120 - Geliştirici
- Huseyin Erekmen - Öğrenci No: 251307099 - Geliştirici

## Tarihler
- Proje Başlangıç Tarihi: 25 Kasım 2025
- Proje Tamamlanış Tarihi: ?

## Problemin Tanımı
Günümüz dijital dünyasında bireyler ve topluluklar arasındaki ilişkiler (sosyal ağlar) giderek karmaşıklaşmaktadır. Milyonlarca bağlantının olduğu bir yapıda;
* İki kişi arasındaki en kısa iletişim yolunu bulmak,
* Ağdaki en etkili kişiyi tespit etmek,
* Birbirinden kopuk toplulukları ayrıştırmak,
gibi problemler çıplak gözle veya basit listelerle çözülemez hale gelmiştir. Bu verilerin anlamlandırılabilmesi için düğüm (node) ve kenar (edge) tabanlı görselleştirme tekniklerine ve algoritmik analizlere ihtiyaç duyulmaktadır.

## Projenin Amacı
**graphSNA** projesinin temel amacı; **Nesne Yönelimli Programlama (OOP)** prensiplerine sıkı sıkıya bağlı kalarak, kullanıcıların dinamik olarak yönetilebildiği ve analiz edilebildiği interaktif bir sistem tasarlamaktır.

Uygulama, kullanıcı özelliklerine (aktiflik, etkileşim sayısı vb.) dayalı **dinamik ağırlık hesaplaması** yaparak standart graflardan farklılaşır. Aşağıdaki temel yetenekleri sunmayı hedefler:
* **Görselleştirme:** Kullanıcıların ve bağlantıların bir tuval (canvas) üzerinde çizilmesi.
* **Yol Analizi:** Dijkstra ve A* algoritmaları ile en maliyetsiz rotaların bulunması.
* **Topluluk Analizi:** Merkezilik (Centrality) hesapları ve BFS/DFS ile ağın taranması.
* **Gruplama:** Welsh-Powell algoritması ile ayrık toplulukların renklendirilerek görselleştirilmesi.
* **Veri Kalıcılığı:** Ağ yapısının JSON/CSV formatında saklanması ve taşınabilmesi.

---