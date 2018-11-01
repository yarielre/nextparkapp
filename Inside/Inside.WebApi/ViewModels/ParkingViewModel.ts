
module Inside.WebApi.ViewModels {

    // $Classes/Enums/Interfaces(filter)[template][separator]
    // filter (optional): Matches the name or full name of the current item. * = match any, wrap in [] to match attributes or prefix with : to match interfaces or base classes.
    // template: The template to repeat for each matched item
    // separator (optional): A separator template that is placed between all templates e.g. $Properties[public $name: $Type][, ]

    // More info: http://frhagn.github.io/Typewriter/

    
    export class ParkingViewModel {
        
        // PRICE
        public price: number = 0;
        
        // PARKINGCATEGORYID
        public parkingCategoryId: number = 0;
        
        // LATITUDE
        public latitude: string = null;
        
        // LONGITUDE
        public longitude: string = null;
        
        // MARKERURL
        public markerUrl: string = null;
        
        // ISRENTED
        public isRented: boolean = false;
        
        // STATUS
        public status: string = null;
        
        // CATEGORY
        public category: string = null;
        
        // RENTINFO
        public rentInfo: Order = null;
        
    }
}