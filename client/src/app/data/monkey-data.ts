export interface Monkey {
  id: number;
  name: string;
  species: string;
  age: number;        // in years
  weightKg: number;   // in kilograms
  arrivalDate: Date;
}

export const MONKEY_DATA: Monkey[] = [
  { id: 1, name: 'George', species: 'Capuchin', age: 5, weightKg: 4.2, arrivalDate: new Date('2024-11-12') },
  { id: 2, name: 'Luna', species: 'Spider Monkey', age: 3, weightKg: 5.1, arrivalDate: new Date('2024-08-22') },
  { id: 3, name: 'Bongo', species: 'Howler Monkey', age: 7, weightKg: 8.3, arrivalDate: new Date('2024-06-05') },
  { id: 4, name: 'Max', species: 'Squirrel Monkey', age: 2, weightKg: 3.7, arrivalDate: new Date('2024-12-01') },
  { id: 5, name: 'Bella', species: 'Marmoset', age: 4, weightKg: 2.9, arrivalDate: new Date('2024-10-15') },
  { id: 6, name: 'Oscar', species: 'Mandrill', age: 6, weightKg: 14.5, arrivalDate: new Date('2024-09-30') },
  { id: 7, name: 'Coco', species: 'Baboon', age: 8, weightKg: 12.1, arrivalDate: new Date('2024-07-18') },
  { id: 8, name: 'Kiki', species: 'Tamarin', age: 1, weightKg: 1.8, arrivalDate: new Date('2024-12-20') },
  { id: 9, name: 'Simba', species: 'Capuchin', age: 9, weightKg: 5.6, arrivalDate: new Date('2024-05-02') },
  { id: 10, name: 'Tiki', species: 'Spider Monkey', age: 2, weightKg: 4.9, arrivalDate: new Date('2024-11-28') },
  { id: 11, name: 'George', species: 'Capuchin', age: 5, weightKg: 4.2, arrivalDate: new Date('2024-11-12') },
  { id: 12, name: 'Luna', species: 'Spider Monkey', age: 3, weightKg: 5.1, arrivalDate: new Date('2024-08-22') },
  { id: 13, name: 'Bongo', species: 'Howler Monkey', age: 7, weightKg: 8.3, arrivalDate: new Date('2024-06-05') },
  { id: 14, name: 'Max', species: 'Squirrel Monkey', age: 2, weightKg: 3.7, arrivalDate: new Date('2024-12-01') },
  { id: 15, name: 'Bella', species: 'Marmoset', age: 4, weightKg: 2.9, arrivalDate: new Date('2024-10-15') },
  { id: 16, name: 'Oscar', species: 'Mandrill', age: 6, weightKg: 14.5, arrivalDate: new Date('2024-09-30') },
  { id: 17, name: 'Coco', species: 'Baboon', age: 8, weightKg: 12.1, arrivalDate: new Date('2024-07-18') },
  { id: 18, name: 'Kiki', species: 'Tamarin', age: 1, weightKg: 1.8, arrivalDate: new Date('2024-12-20') },
  { id: 19, name: 'Simba', species: 'Capuchin', age: 9, weightKg: 5.6, arrivalDate: new Date('2024-05-02') },
  { id: 20, name: 'Tiki', species: 'Spider Monkey', age: 2, weightKg: 4.9, arrivalDate: new Date('2024-11-28') },
];
