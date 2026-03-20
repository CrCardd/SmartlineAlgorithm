# Greedy SmartlineAlgorithm

SmartlineAlgorithm is a greedy solver for a production scheduling problem involving a group of machines, each composed of multiple cells.

### Problem
Setup organization is manual, time-consuming, and constrained by multiple operational rules.

### Objective
Provide an algorithm that generates a daily schedule, defining how each machine must be configured to meet demand.

### Context
Each machine can operate in three modes:
- **misc**
- **dedicated**
- **default**

The optimal mode depends on the set of products assigned to the machine. Each product has a different processing time, and the selected mode directly impacts total production time (e.g., setup time, throughput).

### Algorithm Details
The `Solver` function receives a decision strategy (philosopher) responsible for prioritization.

- **Aristoteles**  
  Maximizes production quantity.  
  Tends to generate excess, but can be advantageous when multiple demands share the same product, as early production reduces future load.

- **Plank**  
  Minimizes excess production.  
  Attempts to closely match demand quantities, reducing overproduction at the cost of potentially less efficient batching.

### Notes
- The algorithm follows a **greedy heuristic**, making local decisions at each step.
- Current implementation operates at the **demand level**, which may cause inefficiencies when multiple demands reference the same product.

## !On going!

- [ ] Dynamic cores per cell  
- [ ] Dynamic unit of measure (setup cooldown & production time)  
- [ ] Bottleneck ("gargalo") handling  
- [ ] Additional decision strategies  