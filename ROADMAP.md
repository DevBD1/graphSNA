# 🗺️ Proje Yol Haritası: graphSNA (Sosyal Ağ Analizi Uygulaması)

Bu doküman, Yazılım Geliştirme Laboratuvarı I projesi için 2 kişilik ekibin takip edeceği gelişim sürecini kapsar. 
Süreç, proje gereksinimlerine ve teslim tarihlerine göre yapılandırılmıştır.

---

## FAZ 1: Hazırlık ve Öğrenme (Tahmini Süre: 1 Hafta)
*Amaç: Projeye kod yazmaya başlamadan önce gerekli teorik altyapıyı ve araç kullanımını oturtmak.*

### 1.1. Teknik Altyapı ve Araçlar
- ✅ **GitHub Kurulumu:** Repo oluşturulacak, `master` ve `test` branch'leri açılacak.
- ✅ **IDE Kurulumu:** Visual Studio (C# WinForms) kurulumu.
- [ ] **Markdown & Mermaid:** Raporlama için Markdown sözdizimi ve Mermaid.js ile diyagram çizimi öğrenilecek.

### 1.2. Graf Teorisi (Graph Theory) Temelleri
- ✅ **Temel Kavramlar:** Düğüm (Node), Kenar (Edge), Komşuluk Listesi (Adjacency List) kavramları araştırılacak.
- ✅ **Graf Türleri:** Yönsüz (Undirected) ve Ağırlıklı (Weighted) grafların mantığı kavranacak.

### 1.3. Nesne Yönelimli Programlama (OOP) Tasarımı
- [ ] **Mimari:** `Node`, `Edge`, `Graph` sınıfları ve `IAlgorithm` arayüzü kağıt üzerinde tasarlanacak.
- [ ] **Kural:** OOP prensiplerine (Kapsülleme, Kalıtım, Polimorfizm) uygunluk şartı incelenecek.

### 1.4. Algoritmaların Mantığı
Aşağıdaki algoritmaların çalışma mantığı ve pseudocode'ları incelenecek:
- [ ] BFS (Breadth-First Search) ve DFS (Depth-First Search).
- [ ] Dijkstra ve A* (En Kısa Yol).
- [ ] Welsh-Powell (Graf Renklendirme).
- [ ] Degree Centrality (Merkezilik Hesabı).

---

## FAZ 2: Geliştirme Süreci (Tahmini Süre: 5 Hafta)
*Amaç: Çalışan, görsel arayüze sahip ve isterleri karşılayan uygulamayı geliştirmek.*

### Adım 2.1: İskelet Yapı (Backend)
*Görselleştirme olmadan temel veri yapısının kurulması.*
- [ ] **Sınıf Yapısı:** `Node` (ID, Ad, Özellikler) ve `Edge` sınıflarının kodlanması.
- [ ] **Graf Yönetimi:** Düğüm ekleme, silme ve Kenar ekleme fonksiyonlarının yazılması.
- [ ] **Hata Yönetimi:** Aynı düğümün tekrar eklenmesinin veya kendine bağlantı (self-loop) yapılmasının engellenmesi.

### Adım 2.2: Veri Yönetimi ve Ağırlık Hesabı
- [ ] **Dinamik Ağırlık Formülü:** İki düğüm arasındaki özellik farklarına (Aktiflik, Etkileşim vb.) dayalı ağırlık hesaplama formülünün `Edge` sınıfına entegre edilmesi.
- [ ] **JSON İşlemleri:** Graf yapısının JSON formatında kaydedilmesi (Export) ve geri yüklenmesi (Import).
- [ ] **Kalıcılık:** Program kapandığında verilerin kaybolmaması için Save/Load yapısının testi.

### Adım 2.3: Görselleştirme (GUI)
- [ ] **Canvas (Tuval):** Düğümlerin daire, kenarların çizgi olarak ekrana çizdirilmesi.
- [ ] **Etkileşim:** Düğümlere tıklandığında özelliklerin (ID, Ad, Puanlar) bir panelde gösterilmesi.
- [ ] **Listeleme:** Düğüm ve kenarların arayüzde liste olarak da görülebilmesi.

### Adım 2.4: Algoritmaların Entegrasyonu
Her algoritma ayrı butonla tetiklenecek ve sonucu görsel/tablo olarak sunulacak.
- [ ] **Gezinme:** BFS ve DFS ile erişilebilir düğümlerin bulunması.
- [ ] **En Kısa Yol:** Seçilen iki düğüm arası Dijkstra ve A* ile yolun çizilmesi ve maliyetin gösterilmesi.
- [ ] **Analiz:**
    - [ ] Bağlı bileşenlerin tespiti.
    - [ ] En etkili 5 kullanıcının (Degree Centrality) tablo ile gösterimi.
- [ ] **Renklendirme:** Welsh-Powell algoritması ile komşu düğümlerin farklı renklere boyanması.

### Adım 2.5: Test ve Raporlama
- [ ] **Performans Testi:** 50-100 düğümlü graflar oluşturulup algoritmaların hızının test edilmesi.
- [ ] **Rapor Yazımı:** GitHub `README.md` dosyasına proje raporunun yazılması.
    - [ ] Sınıf Diyagramları (Mermaid).
    - [ ] Algoritma Akış Şemaları (Mermaid).
    - [ ] Ekran görüntüleri ve test sonuçları.

---

## Görev Dağılımı (2 Geliştirici)

### Geliştirici A:

Odak: Temel Yapı ve Gezinti Algoritmaları.

Görevler:
- Node, Edge ve Graph temel sınıflarını yazmak.
- JSON Dosya Okuma/Yazma işlemleri.
- BFS ve DFS Algoritmaları.
- Bağlı Bileşenleri (Connected Components) Bulma.

### Geliştirici B:

Odak: Hesaplamalı Algoritmalar ve Görselleştirme Mantığı.

Görevler:
- Ağırlık Hesaplama Formülü (Projedeki özel formül).
- Dijkstra ve A* (En Kısa Yol) Algoritmaları.
- Welsh-Powell Renklendirme Algoritması.
- Merkezilik (Degree Centrality) Hesabı.

Ortak Görev: Arayüz tasarımı ve olayları (tıklama, sürükleme) beraber veya dönüşümlü yapılmalıdır.

## ✅ Teslim Kontrol Listesi
- [ ] Kodlar OOP prensiplerine tam uygun mu? 
- [ ] GitHub'da her iki üyenin de eşit/orantılı katkısı var mı? 
- [ ] Rapor (Markdown) eksiksiz mi? 
- [ ] Proje sıkıştırılmış tek dosya (Kod + Rapor) halinde hazır mı? 
