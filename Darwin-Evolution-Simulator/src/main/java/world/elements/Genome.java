package world.elements;

import world.basic.MapDirection;

import java.util.Arrays;
import java.util.Random;

public class Genome {

    private final static Random generator = new Random();
    public short [] dna;

    public Genome(short [] initialDNA){
        this.dna = initialDNA;
        completeDNA();
    }

    public Genome(int dnaLength){
        this(createRandomDNA(dnaLength));
    }

    public Genome(int dnaLength, Genome genome1, Genome genome2){ ;
        int cut1 = generator.nextInt(dnaLength);
        int cut2 = generator.nextInt(dnaLength);
        this.dna = new short[dnaLength];
        for (int i=0; i<Math.min(cut1, cut2); i++)
            dna[i] = genome1.getDna()[i];
        for (int i=Math.min(cut1, cut2); i<Math.max(cut1, cut2); i++)
            dna[i] = genome2.getDna()[i];
        for (int i=Math.max(cut1, cut2); i<dnaLength; i++)
            dna[i] = genome1.getDna()[i];
        completeDNA();
    }

    private static short [] createRandomDNA(int length){
        short [] dna = new short[length];
        for (int i=0; i<length; i++)
            dna[i] = (short) generator.nextInt(MapDirection.values().length);
        return dna;
    }

    private void completeDNA(){
        int [] buckets = new int[MapDirection.values().length];
        for (short gen : dna)
            buckets[gen]++;
        for (int i=0; i<buckets.length; i++){
            if (buckets[i] == 0){
                int genToReduce;
                do{
                    genToReduce = generator.nextInt(buckets.length);
                } while (buckets[genToReduce] < 2);
                buckets[genToReduce] --;
                buckets[i] ++;
            }
        }
        int last=0;
        for (short i=0; i<buckets.length; i++){
            for (int j=last; j<last+buckets[i]; j++)
                dna[j] = i;
            last += buckets[i];
        }
    }

    @Override
    public boolean equals(Object o) {
        if (this == o) return true;
        if (o == null || getClass() != o.getClass()) return false;
        Genome genome = (Genome) o;
        return Arrays.equals(dna, genome.dna);
    }

    @Override
    public int hashCode() {
        return Arrays.hashCode(dna);
    }

    public short [] getDna(){
        return dna.clone();
    }

    @Override
    public String toString() {
        StringBuilder genotype = new StringBuilder();
        for (short g : dna){
            genotype.append(String.valueOf(g));
        }
        return genotype.toString();
    }
}

