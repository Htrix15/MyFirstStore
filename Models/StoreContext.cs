using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using MyFirstStore.Services;
using MyFirstStore.Models;
using MyFirstStore.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MyFirstStore.Models
{
    public class StoreContext : DbContext
    {
        public DbSet<ProductsType> ProductsTypes { get; set; }
        public DbSet<ProductsAttribute> ProductsAttributes { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<AttributesToProduct> AttributesToProducts { get; set; }
        public DbSet<AttributesToProductType> AttributesToProductTypes { get; set; }
        public DbSet<ProductsPicturesPath> ProductsPicturesPaths { get; set; }
        public DbSet<OrderStatus> OrderStatuses { get; set; }
        public DbSet<Order> Orders { get; set; } 
        public DbSet<OrderPosition> OrderPositions { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AttributesToProduct>().HasKey(table => new {table.ProductsAttributeId,  table.ProductId});
            modelBuilder.Entity<AttributesToProductType>().HasKey(table => new { table.ProductsAttributeId, table.ProductsTypeId});
            modelBuilder.Entity<ProductsPicturesPath>().HasKey(table => new { table.ProductId, table.NPicture});
            modelBuilder.Entity<OrderPosition>().HasKey(table => new { table.OrderId, table.ProductId});

           modelBuilder.Entity<ProductsAttribute>().HasData(
                new ProductsAttribute[]
                {
                    new ProductsAttribute{Id =1, Name="Default" },
                    new ProductsAttribute{Id =3, Name="Высота" },
                    new ProductsAttribute{Id =4, Name="Ширина" },
                    new ProductsAttribute{Id =5, Name="Вес" },
                    new ProductsAttribute{Id =6, Name="Год создания" },
                    new ProductsAttribute{Id =8, Name="Оригинал" }
                });
            modelBuilder.Entity<ProductsType>().HasData(
                new ProductsType[]
                {
                    new ProductsType(){Id=1,Name="Это родительский тип",ParentProductTypeId=1,HexColor="fcba03" },
                    new ProductsType(){Id=2,Name="Без родителя",ParentProductTypeId= 1,HexColor="fcba03" },
                    new ProductsType(){Id=3,Name="Картины",ParentProductTypeId= 1,HexColor="0e2852" },
                    new ProductsType(){Id=4,Name="Малевич",ParentProductTypeId= 3,HexColor="214c91" },
                    new ProductsType(){Id=5,Name="Кандинский",ParentProductTypeId= 3,HexColor="3473d9" },
                    new ProductsType(){Id=6,Name="Статуи",ParentProductTypeId= 1,HexColor="0b7316" },
                    new ProductsType(){Id=7,Name="Неизвестный",ParentProductTypeId= 6,HexColor="15ad25" }
                });
            modelBuilder.Entity<AttributesToProductType>().HasData(
                new AttributesToProductType[]
                {
                      new AttributesToProductType{ProductsTypeId=3, ProductsAttributeId= 1},
                      new AttributesToProductType{ProductsTypeId=3, ProductsAttributeId= 3},
                      new AttributesToProductType{ProductsTypeId=3, ProductsAttributeId= 4},
                      new AttributesToProductType{ProductsTypeId=3, ProductsAttributeId= 6},
                      new AttributesToProductType{ProductsTypeId=4, ProductsAttributeId= 1},
                      new AttributesToProductType{ProductsTypeId=5, ProductsAttributeId= 1},
                      new AttributesToProductType{ProductsTypeId=5, ProductsAttributeId= 8},
                      new AttributesToProductType{ProductsTypeId=6, ProductsAttributeId= 1},
                      new AttributesToProductType{ProductsTypeId=6, ProductsAttributeId= 5},
                      new AttributesToProductType{ProductsTypeId=6, ProductsAttributeId= 6},
                      new AttributesToProductType{ProductsTypeId=7, ProductsAttributeId= 1},
                });
            modelBuilder.Entity<Product>().HasData(
                new Product[]
                    {
                       new Product{Id=1,
                                   ProductsTypeId=4,
                                   Name="Черный квадрат",
                                   Price=10000,
                                   Sale=false,
                                   SalePrice=10000,
                                   CountToStore=1,
                                   DeliveryToStoreDay=120,
                                   MainAttribute="Черный и квадратный",
                                   Description="«Чёрный квадрат» входит в цикл супрематических работ Казимира Малевича, в которых художник исследовал базовые возможности " +
                                   "цвета и композиции; является, по замыслу, частью триптиха, в составе которого также присутствуют «Чёрный круг» и «Чёрный крест».",
                                   MainPicturePath="/images/Картины/Малевич/Черный_квадрат/чк.png",
                                   AvailableOnRequest=true,
                                   AvailableMaxCountOnRequest=2,
                                   AvailableMaxCountOnRequestOLD=0,
                                   CountToStoreOLD=0},
                       new Product{
                                    Id=2,
                                    ProductsTypeId= 4,
                                    Name="Супремус 56",
                                    Price=5000,
                                    Sale=true,
                                    SalePrice=2500,
                                    CountToStore=0,
                                    DeliveryToStoreDay=120,
                                    MainAttribute="Менее известная работа",
                                    Description="Эта картина создает ощущение легкости, воздушности. Много белого пространства, легкие фигуры разных цветов, все создает ощущение полета, " +
                                    "оторванности от земли. Легкие на вид фигуры парят в белом пространстве. Ощущается как будто бы легкое движение – сказывается опыт художника во время " +
                                    "работ с футуристическими картинами и различных вариантов исполнения супрематических картин.",
                                    MainPicturePath="/images/Картины/Малевич/Супремус_56/1.PNG",
                                    AvailableOnRequest=true,    
                                    AvailableMaxCountOnRequest=2,
                                    AvailableMaxCountOnRequestOLD=0,
                                    CountToStoreOLD=0},
                        new Product{Id=3,
                                    ProductsTypeId= 4,
                                    Name="Коллаж",
                                    Price=2500,
                                    Sale=false,
                                    SalePrice=2500,
                                    CountToStore=5,
                                    DeliveryToStoreDay=5,
                                    MainAttribute="Из разных работ",    
                                    Description="Казимир Малевич всегда радовал своими шедеврами всех людей, которые были неравнодушны к супрематизму. Если углубляться в предисловие его картин и" +
                                    " конкретно данного произведения искусства, то стоит отметить, что Малевич смог изобрести особенную и уникальную в своем роде и видении теорию абстрактного" +
                                    " искусства, которое собственно и называется супрематизм. Сама задумка заключается в том, что все композиции художника, выполненные в стиле супрематизм," +
                                    " изначально нацелены на то, чтобы показать раскрепощение автора, из подражательного подчинения какой либо вещи, к естественному раскрытию творчества.",
                                    MainPicturePath="/images/Картины/Малевич/Коллаж/upload-15-pic4_zoom-1500x1500-99014.jpg",
                                    AvailableOnRequest=true,
                                    AvailableMaxCountOnRequest=0,
                                    AvailableMaxCountOnRequestOLD=0,
                                    CountToStoreOLD=0},
                        new Product{Id=4,
                                    ProductsTypeId= 5,
                                    Name="Композиция VIII",
                                    Price=5000,
                                    Sale=false,
                                    SalePrice=5000,
                                    CountToStore=0,
                                    DeliveryToStoreDay=120,
                                    MainAttribute="Холст, масло",
                                    Description="Зритель был совершенно шокирован переходом от апокалиптических эмоций Седьмой композиции к геометрическому ритму Восьмой. Написанная десятью годами позже," +
                                    " в 1923-м, она представляет собой логическое развитие творческого гения художника и, конечно, в определенной степени отражает влияние супрематизма и конструктивизма," +
                                    " впитанного Кандинским в России и в Баухаусе.",
                                    MainPicturePath="/images/Картины/Кандинский/Композиция_VIII/unnamed.jpg",
                                    AvailableOnRequest=false,
                                    AvailableMaxCountOnRequest=1,
                                    AvailableMaxCountOnRequestOLD=0,    
                                    CountToStoreOLD=0},
                        new Product{Id=5,
                                    ProductsTypeId= 7,
                                    Name="Римлянин",
                                    Price=40000,
                                    Sale=true,
                                    SalePrice=60000,
                                    CountToStore=5,
                                    DeliveryToStoreDay=15,
                                    MainAttribute="Знатный римлянин",
                                    Description="Знатный римлянин",
                                    MainPicturePath="/images/Статуи/Неизвестный/Римлянин/Escultura-germano.jpg",
                                    AvailableOnRequest=true,
                                    AvailableMaxCountOnRequest=10,
                                    AvailableMaxCountOnRequestOLD=0,
                                    CountToStoreOLD=0}
                        });
            modelBuilder.Entity<AttributesToProduct>().HasData(
                new AttributesToProduct []
                {
                    new AttributesToProduct{ProductId=1, ProductsAttributeId= 1 , Value=""},
                    new AttributesToProduct{ProductId=2, ProductsAttributeId= 1 , Value=""},
                    new AttributesToProduct{ProductId=3, ProductsAttributeId= 1 , Value=""},
                    new AttributesToProduct{ProductId=4, ProductsAttributeId= 1 , Value=""},
                    new AttributesToProduct{ProductId=5, ProductsAttributeId= 1 , Value=""},
                    new AttributesToProduct{ProductId=1, ProductsAttributeId= 3 , Value="79,5"},
                    new AttributesToProduct{ProductId=2, ProductsAttributeId= 3 , Value="71"},
                    new AttributesToProduct{ProductId=3, ProductsAttributeId= 3 , Value=""},
                    new AttributesToProduct{ProductId=4, ProductsAttributeId= 3 , Value="140"},
                    new AttributesToProduct{ProductId=1, ProductsAttributeId= 4 , Value="79,5"},
                    new AttributesToProduct{ProductId=2, ProductsAttributeId= 4 , Value="80,5"},
                    new AttributesToProduct{ProductId=3, ProductsAttributeId= 4 , Value="NULL"},
                    new AttributesToProduct{ProductId=4, ProductsAttributeId= 4 , Value="201"},
                    new AttributesToProduct{ProductId=1, ProductsAttributeId= 6 , Value="1915"},
                    new AttributesToProduct{ProductId=2, ProductsAttributeId= 6 , Value="1916"},
                    new AttributesToProduct{ProductId=3, ProductsAttributeId= 6 , Value="Новодел"},
                    new AttributesToProduct{ProductId=4, ProductsAttributeId= 6 , Value=""},
                    new AttributesToProduct{ProductId=4, ProductsAttributeId= 8 , Value="Нет"}
                });
            modelBuilder.Entity<ProductsPicturesPath>().HasData(
                new ProductsPicturesPath[]{
                     new ProductsPicturesPath(){ProductId=1,NPicture= 0,PicturePath=null},
                     new ProductsPicturesPath(){ProductId=2,NPicture= 0,PicturePath="/images/Картины/Малевич/Супремус_56/1.PNG"},
                     new ProductsPicturesPath(){ProductId=2,NPicture= 1,PicturePath="/images/Картины/Малевич/Супремус_56/2.PNG"},
                     new ProductsPicturesPath(){ProductId=2,NPicture= 2,PicturePath="/images/Картины/Малевич/Супремус_56/3.PNG"},
                     new ProductsPicturesPath(){ProductId=3,NPicture= 0,PicturePath=""},
                     new ProductsPicturesPath(){ProductId=3,NPicture= 1,PicturePath=null},
                     new ProductsPicturesPath(){ProductId=3,NPicture= 2,PicturePath=null},
                     new ProductsPicturesPath(){ProductId=4,NPicture= 0,PicturePath="/images/Картины/Кандинский/Композиция_VIII/4.PNG"},
                     new ProductsPicturesPath(){ProductId=4,NPicture= 1,PicturePath="/images/Картины/Кандинский/Композиция_VIII/5.PNG"},
                     new ProductsPicturesPath(){ProductId=4,NPicture= 2,PicturePath="/images/Картины/Кандинский/Композиция_VIII/7.PNG"},
                     new ProductsPicturesPath(){ProductId=5,NPicture= 0,PicturePath=null}
            });
        }  
        public StoreContext(DbContextOptions<StoreContext> options)
            : base(options)
        {  

        }



    }
}
