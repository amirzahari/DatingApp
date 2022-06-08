
let data: number | string = 42;
data = 45
data = "45"

// *** create const property type to prevent mistake
// *** datatype safety

interface Car {
    color : string;
    model : string;
    topSpeed? : number;
}

const car1: Car = {
    color: "Black",
    model: "BMW"
}

const car2: Car = {
    color: "red",
    model: "mercedes",
    topSpeed: 180
}

// *** can make mistake if undependend with cons fix
const carwithoutbind = {
    color: "blue",
    model: "VW",
    topSpeed: "180",

}

const mult1 = (x: number, y: number) => {
    x * y;
}

const mult2 = (x, y) => {
    x * y;
}

