# NpcEditor
Bombom'da NPC'lerin hitboxlarını düzenlemek oldukça karmaşık bir iştir. Bunlardan birini düzeltmek istediğinizde x, y, genişlik, uzunluk için yoğun hesaplamalar yapmak zorunda kalırsınız.
Düzenleme işlemlerini kolaylaştırmak için bu uygulamayı geliştirdim. Bu uygulama ile birlikte NPC alanlarını kusursuz şekilde ayarlayabilir, düzenlerken önizleyebilirsiniz.

Uygulama, karakterin kendine özgü animasyonlarını yükleyerek, üzerinde alan seçimi yapabilmeyi sağlar ve seçtikten sonra yeni koordinatları karakterin üzerine yerleştirerek yeni hitbox alanını gösterir.

Uygulamanın işlevleri hitbox düzenlemekle sınırlı değil. Karakterlerin animasyon isimlerini öğrenmek veya bu animasyonların hareketini izlemek için de bu aracı kullanabilirsiniz.

Harita görünümü ile karakterlerin harita üzerindeki duruşunu görebilirsiniz. Koordinat belirlemek için istediğiniz bir noktaya taşıyabilirsiniz. 

Kullanım videosu
----------------
[![Kullanım videosu](https://img.youtube.com/vi/gXL1wEztdQI/0.jpg)](https://www.youtube.com/watch?v=gXL1wEztdQI)


Özellikler
----------
- Veritabanından NPC listeleme, isim veya NPC ID'si yazarak arama yapılabilir.
- Seçilen NPC'nin ID ve Script ve hitbox değerlerini öğrenilir.
- Resource üzerinden NPC swf'si otomatik tespit edilerek yüklenir.
- NPC animasyonlarını listelenebilir.
- NPC animasyonu oynatılabilir.
- Çoklu seçim özelliğini açarak aynı ModelID'ye sahip NPC'lerin hitboxunu toplu değiştirebilirsiniz. 
- NPC hitboxunu görüntüleyebilir ve düzenleme yapabilirsiniz.
- Harita yükleyerek NPC'lerin görünüşünü orada test edebilirsiniz.
- NPC'yi harita üzerinde bir noktaya taşıyabilir, konumunu öğrenebilirsiniz.
- Oyuncuların harita üzerindeki doğum yerleri görüntülenebilir. 


Nasıl kullanılır?
-----------------
### Yapılandırma
Programı çalıştırmadan önce `NpcEditor.exe.config` dosyasındaki SQL bağlantı cümlesi ve resource adresi düzenlenmelisiniz. Resource adresi, internet adresi de olabilir cihazınızdaki bir yerel klasör de.
Eğer resource dosyalarınız var ise dosyaların daha hızlı yüklenmesi için yerelde kullanmanızı tavsiye ederim.

Örnek resource adresleri:
- Local: C:\Server\Resource  (kendi adresinizi yazınız)
- TR: http://ddttr-a.akamaihd.net/
- BR: http://ddt-a.akamaihd.net/

### NPC seçimi
NPC'ler veritabanından yüklenerek sol altta listeleniyor. Sol üstteki bölümde **ID** veya **isim** yazarak listede arama yapabilirsiniz. Daha ileride daha kapsamlı bir arama bölümü ekleyebilirim.

### NPC Action Movie
Karakterlerin animasyonları her NPC'de aynı değildir. Bu karakterlerin kendine özgü action isimleri living swf'sinden otomatik yükleniyor. Action Listesinden seçerek oynatabilirsiniz. 
Bir tavsiye olarak, hitbox düzenlerken daha doğru sonuç almak için **cry** animasyonunu izlemenizi tavsiye ederim.
Vurulduğunda küçülen karakterler olduğu için vurulduğunda oynatılan bu animasyon, daha gerçekçi bir alan seçmenizi sağlar. 

NOT: Sunucu geliştiriyorsanız NPC scriptlerinde bu action isimlerine çokça ihtiyaç duyarsınız. Listedeki isimleri server projenizde PlayMovie fonksiyonlarında kullanabilirsiniz.

### NPC Hitbox Seçimi
NPC'yi yeni seçtiğinizde db'deki mevcut hitboxlar gözükür. Fare ile animasyon üzerinde bir noktaya tıklayıp dikdörtgen çizerek hitbox alanını kolayca belirleyebilirsiniz. İnce ayar yapmak için sağ alttaki hitbox `x`, `y`, `width`, `height` değerlerini değiştirebilirsiniz. Geçici seçiminizi temizlemek ve kayıtlı haline döndürmek için `sıfırla` butonuna basabilirsiniz. Seçimi bitirdiğinizde `kaydet`meyi unutmayın.

NPC'nin animasyonunun merkez noktasına bir nokta işaretledim. Bu nokta size bazen yardımcı olacaktır. NPC'nin ölçüleri, bu nokta baz alınarak hesaplanır.

Çoklu seçim özelliğini etkinleştirirseniz liste üzerinden birden fazla NPC'yi seçerek tek sefer her birine aynı hitbox değerlerini kaydedebilirsiniz. Bu sayede farklı zorluk derecelerine ait NPC'leri ayrı ayrı düzenlemeye uğraşmazsınız. Burada temel şart olarak karakterlerin aynı modeli kullanmasıdır. Yanlış bir karaktere aynı hitboxu atamayı önlemek için ModelID'si farklı olan karakterleri listeden seçemezsiniz.  

### NPC Taşıma-Sürükleme
CTRL + fare ile sol tıkladıktan sonra fare hareketiyle NPC'yi ekranda istediğiniz noktaya sürükleyebilirsiniz. Sol tıkı bıraktığınız zaman karakteri mevcut konumunda sabitlersiniz.

### Harita seçimi
Bu özellik opsiyoneldir. Karakterleri istediğiniz bir harita üzerinde görmek için kullanablirsiniz. Veribanınından yüklenen haritalarınız otomatik olarak listelenir. Listeden bir harita seçtikten sonra `Harita yükle` butonuna bastıktan sonra harita görselleri ve haritadaki doğum pozisyonları gösterilir.

Nasıl çalışır?
--------------
Uygulama iki parçadan oluşur. Ana işlemler NpcEditor adlı C# projesinde yapılır. Living SWF'lerinin işlenmesi için flash player kullandım. Oynatmak için Flash.ocx dosyasını kullanıyorum.

NPC animasyonları, dikdörtgen seçimi gibi işlemler NpcLoader adını verdiğim yardımcı flash projesinde gerçekleştiriliyor.

Bu iki ayrı proje, flash call çağrılarıyla haberleşiyor. 

Geliştirme
----------
Ana uygulama C# ile geliştirildi. Animasyonlar için Actionscript kullanarak yardımcı bir araç tasarladım.

C# projesini **Visual Studio 2019** ile geliştirebilirsiniz. Uygulama flash playera ihtiyaç duyar. Flash nesnesi güncel VS sürümlerinde sorun çıkardığı için VS 2019'dan ileri sürümleri kullanmamanız gerekebilir.

NpcLoader adlı yardımcı swf projesini derlemek için **FlashDevelop**'u kullandım. Geliştirmek istiyorsanız olası hataları, logları görmek için mevcut Flash.ocx'i debugger versiyonuyla değiştirmeniz gerekir.

Katkıda bulunanlar
-------
Tüm geliştirme süreci tarafımca (hydrogen31) yapılmıştır. Uygulamayı tasarlarken Zephyr'in [video](https://www.youtube.com/watch?v=W3OLUQuxwG8)sundan ilham aldım. Uygulama fikrini aklıma sokan Eren'e de selam olsun.